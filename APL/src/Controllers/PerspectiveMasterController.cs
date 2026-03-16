using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APL.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PerspectiveMasterController : Controller
    {

        // GET: PerspectiveMaster/Get
        public ActionResult Get()
        {
            return View();
        }

        // GET: PerspectiveMaster/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PerspectiveMaster/Delete
        [HttpPost]
        public ActionResult Delete(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PerspectiveMaster/Edit
        public ActionResult Edit()
        {
            return View();
        }

        
    }
}
