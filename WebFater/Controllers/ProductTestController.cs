using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebFater.Controllers
{
    [ApiController]
    [Route("Product")]
    public class ProductTestController : ControllerBase
    {
        private TestProductService _productService;
        public ProductTestController(TestProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getProducts")]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            try
            {
                var result = await _productService.GetProductsTest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetProductById")]
        public async Task<ActionResult<List<Product>>> GetProduct(long Id)
        {
            try
            {
                var result = await _productService.GetProductTest(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetBrands")]
        public async Task<ActionResult<List<ProductBrand>>> GetBrands()
        {
            try
            {
                var result = await _productService.GetProductBrandTest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
