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
    public class PerspectiveMasterService : IPerspectiveMasterService
    {
        private readonly AplDbContext _db;

        public PerspectiveMasterService(AplDbContext db)
        {
            _db = db;
        }

        public Task<int> CreatePerspective(UserCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePerspective(UserCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserManagementDto>> GetAllPerspective()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePerspective(UserCreateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
