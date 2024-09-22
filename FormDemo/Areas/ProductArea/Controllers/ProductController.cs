using Microsoft.AspNetCore.Mvc;
using FormDemo.Areas.ProductArea.Models;
using Microsoft.AspNetCore.Http;

namespace FormDemo.Areas.ProductArea.Controllers
{
   
       
        [Area("ProductArea")]
        public class ProductController : Controller
        {
        [HttpGet]
        [HttpPost]
        public IActionResult CreateProduct()
        {
            return View();
        }

    }

}
