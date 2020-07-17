using System.Collections.Generic;
using DocketManagerSample.Models.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocketManagerSample.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(TestProducts.Products);
        }
    }
}