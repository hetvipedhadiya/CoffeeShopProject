using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormDemo.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Save(StudentModel studentModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else {

            return View("Index",studentModel);
            }
        }
    }
}
