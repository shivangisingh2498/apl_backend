using APL.Data;
using APL.Entities;
using APL.Models;
using APL.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Xml.Linq;
namespace APL.Services
{
    public class TargetSettingsService : ITargetSettingsService
    {
        private readonly AplDbContext _db;

        public TargetSettingsService(AplDbContext db)
        {
            _db = db;
        }



        public async Task<List<TargetTemplateDto>> GetTemplateByRole(string input)
        {
            FinancialYearRange fy = GetFinancialYearRange(DateTime.UtcNow);

            DateTime fyStart = fy.start;
            DateTime fyEnd = fy.end;

            List<int> departmentIds = await _db.tbl_user_management
           .Include(x => x.tbl_roles_master)
           .AsNoTracking()
           .Where(x => x.isactive && !x.isdisable && x.email == input)
           .Select(x => new { x.departmentid, Role = x.tbl_roles_master.roles })
           .Where(x => x.Role == "SPOC")
           .Select(x => x.departmentid) // Extract ONLY the IDs
           .ToListAsync();

            if (!departmentIds.Any())
            {
                return new List<TargetTemplateDto>();
            }

            // 2. Fetch the template where the header matches one of those pairs
            List<TargetTemplateDto> template = await _db.tbl_bsc_form_header
        .AsNoTracking()
        .Where(x =>
            x.isactive && x.issharedbyadmin &&
            x.createdon >= fyStart &&
            x.createdon <= fyEnd &&
            departmentIds.Contains(x.departmentid)
        )
        .Select(x => new TargetTemplateDto
        {
            formId = x.id,
            departmentId = x.departmentid,
            department = x.tbl_department_master.department,
            stationId = x.stationid,
            stationName = x.tbl_station_master.station,
            perspective = x.tbl_bsc_perspective.Select(p => new TargetPerspectiveDto
            {
                perspectiveId = p.perspectiveid,
                perspective = p.tbl_perspective.perspective,
                strategicObjectiveId = p.tbl_bsc_strategic_objective.Select(so => so.strategicobjectiveid).FirstOrDefault(),
                strategicObjective = p.tbl_bsc_strategic_objective.Select(so => so.tbl_strategic_objective.strategicobjective).FirstOrDefault(),
                kpiList = p.tbl_bsc_strategic_objective.SelectMany(so => so.tbl_bsc_kpi).Select(k => new TargetKpiDto
                {
                    kpiId = k.kpiid,
                    frequency = k.tbl_object_master.value, // Ensure this column exists in tbl_bsc_kpi
                    definition = k.tbl_kpi_master.definition,
                    uom = k.tbl_kpi_master.tbl_uom_master.uom,
                    minValue = k.tbl_kpi_master.tbl_uom_master.minvalue.ToString(),
                    maxValue = k.tbl_kpi_master.tbl_uom_master.maxvalue.ToString(),

                    yearlyTarget = k.tbl_bsc_yearly_target.Where(y => y.isactive)
                                   .Select(y => new YearlyTargetDto
                                   {
                                       targetCurrentYear = y.currentyeartarget,
                                       targetNextYear = y.nextyeartarget,
                                       targetNextToNextYear = y.nexttonextyeartarget,
                                       weightage = y.weigthage,
                                       initiative = y.initiative
                                   })
                                   .FirstOrDefault(),

                    monthlyTarget = k.tbl_bsc_monthly_target
                                    .Where(m => m.isactive).OrderBy(m => m.monthno)
            .Select(m => new MonthlyTargetDto
            {
                monthNo = m.monthno,
                targetValue = m.targetvalue
            })
            .ToList()

                }).ToList()
            }).ToList()
        })
        .ToListAsync();
            return template;
        }

        public async Task<FrequencyRuleResponseDto> GetFrequencyRulesAsync(int fyStartYear)
        {
            // 1. Load frequency rules + master values
            List<FrequencyMonthRule> rules = await _db.tbl_frequency_months_mapping
    .Where(x => x.isactive)
    .Select(x => new FrequencyMonthRule
    {
        frequencyId = x.frequencyid,
        frequencyName = x.tbl_object_master.value,
        monthNo = x.monthno
    })
    .ToListAsync();

            FrequencyRuleResponseDto response = new FrequencyRuleResponseDto();
            // 2. Build FY month map
            Dictionary<int, int> fyMonths = GetFinancialYearMonths(fyStartYear)
                            .ToDictionary(x => x.MonthNo, x => x.Year);


            YearlyHeaderDto yearHeader = GetYearlyHeaders(fyStartYear);
            response.yearlyHeaderDto = yearHeader;
            // 3. Group by frequency


            foreach (var group in rules.GroupBy(x => new { x.frequencyId, x.frequencyName }))
            {
                var freqDto = new FrequencyRuleDto
                {
                    frequency = group.Key.frequencyName
                };

                foreach (var month in group
                    .OrderBy(x => x.monthNo < 4)   // ✅ FY ordering
                    .ThenBy(x => x.monthNo))
                {
                    if (!fyMonths.ContainsKey(month.monthNo))
                        continue;

                    int year = fyMonths[month.monthNo];

                    freqDto.months.Add(new MonthDto
                    {
                        id = month.monthNo,
                        value = FormatMonthLabel(month.monthNo, year)
                    });
                }

                response.frequencyRule.Add(freqDto);
            }

            return response;
        }

        public async Task<ResultDto<string>> SaveTemplateTarget(TargetTemplateDto targetSetting)
        {
            BscFormHeader? exists = await _db.tbl_bsc_form_header.Include(x => x.tbl_bsc_kpi)
                .ThenInclude(x => x.tbl_bsc_monthly_target)
                .Include(x => x.tbl_bsc_kpi)
                .ThenInclude(x => x.tbl_bsc_yearly_target).FirstOrDefaultAsync(x => x.id == targetSetting.formId);
            int year = DateTime.UtcNow.Year;

            if (exists != null)
            {
                foreach (TargetPerspectiveDto perspective in targetSetting.perspective)
                {
                    foreach (var kpiDto in perspective.kpiList)
                    {
                        BscYearlyTarget? yearlyTarget =
                            exists.tbl_bsc_kpi
                                .SelectMany(k => k.tbl_bsc_yearly_target)
                                .FirstOrDefault(y =>
                                    y.kpiid == kpiDto.kpiId &&
                                    y.bscformid == targetSetting.formId);

                        if (yearlyTarget == null)
                        {
                            yearlyTarget = new BscYearlyTarget
                            {
                                bscformid = targetSetting.formId,
                                kpiid = kpiDto.kpiId,
                                isactive = true,
                                createdby = "System"
                            };

                            _db.tbl_bsc_yearly_target.Add(yearlyTarget);
                        }

                        // yearlyTarget.pyactual = kpiDto.yearlyTarget?.pyactual;
                        yearlyTarget.currentyeartarget = kpiDto.yearlyTarget.targetCurrentYear;
                        yearlyTarget.nextyeartarget = kpiDto.yearlyTarget.targetNextYear;
                        yearlyTarget.nexttonextyeartarget = kpiDto.yearlyTarget.targetNextToNextYear;
                        yearlyTarget.weigthage = kpiDto.yearlyTarget.weightage;
                        yearlyTarget.initiative = kpiDto.yearlyTarget?.initiative;
                        yearlyTarget.updatedby = "System"; //add eid from token
                        yearlyTarget.updatedon = DateTime.UtcNow;


                        if (kpiDto.monthlyTarget == null)
                            continue;

                        foreach (MonthlyTargetDto monthDto in kpiDto.monthlyTarget)
                        {
                            // ✅ Rule: Save ONLY when monthno != 0
                            if (monthDto.monthNo == 0)
                                continue;

                            BscMonthlyTarget? monthlyTarget =
                                exists.tbl_bsc_kpi
                                    .SelectMany(k => k.tbl_bsc_monthly_target)
                                    .FirstOrDefault(m =>
                                        m.bscformid == targetSetting.formId &&
                                        m.kpiid == kpiDto.kpiId &&
                                        m.monthno == monthDto.monthNo &&
                                        m.year == year);

                            if (monthlyTarget == null)
                            {
                                monthlyTarget = new BscMonthlyTarget
                                {
                                    bscformid = targetSetting.formId,
                                    kpiid = kpiDto.kpiId,
                                    monthno = monthDto.monthNo,
                                    year = year,
                                    isactive = true,
                                    createdby = "System"
                                };

                                _db.tbl_bsc_monthly_target.Add(monthlyTarget);
                            }

                            monthlyTarget.targetvalue = Convert.ToDecimal(monthDto.targetValue);
                            monthlyTarget.updatedby = "System";
                            monthlyTarget.updatedon = DateTime.UtcNow.AddHours(5.5);
                        }

                    }


                }
                _db.SaveChangesAsync();
                return new ResultDto<string>
                {
                    status = "Success",
                    result = "Template saved successfully."
                };
            }
            else
            {
                throw new Exception("Form not found");
            }
        }

        private FinancialYearRange GetFinancialYearRange(DateTime date)
        {
            int fyYear = date.Month >= 4 ? date.Year : date.Year - 1;

            FinancialYearRange range = new FinancialYearRange();
            range.start = DateTime.SpecifyKind(new DateTime(fyYear, 4, 1), DateTimeKind.Utc);
            range.end = DateTime.SpecifyKind(new DateTime(fyYear + 1, 3, 31, 23, 59, 59), DateTimeKind.Utc);

            return range;

        }
        private static List<(int MonthNo, int Year)> GetFinancialYearMonths(int fyStartYear)
        {
            var months = new List<(int, int)>();

            // Apr–Dec (FY start year)
            for (int m = 4; m <= 12; m++)
                months.Add((m, fyStartYear));

            // Jan–Mar (FY + 1)
            for (int m = 1; m <= 3; m++)
                months.Add((m, fyStartYear + 1));

            return months;
        }
        private static string FormatMonthLabel(int month, int year)
        {
            return $"{new DateTime(year, month, 1):MMM}'{year % 100:D2}";
        }

        private static YearlyHeaderDto GetYearlyHeaders(int year)
        {
            YearlyHeaderDto dto = new YearlyHeaderDto
            {
                CurrentYearTargetHeader = "FY'" + (year % 100).ToString("D2"),
                NextYearTargetHeader = "FY'" + ((year + 1) % 100).ToString("D2"),
                NextToNextYearTargetHeader = "FY'" + ((year + 2) % 100).ToString("D2")
            };

            return dto;
        }
    }
}
