using Application.Services;
using Domain.DTOs;
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

            var result = await _productService.GetProductsTest();
            return Ok(result);

        }

        [HttpGet("GetProductById")]
        public async Task<ActionResult<List<Product>>> GetProduct(long Id)
        {

            var result = await _productService.GetProductTest(Id);
            return Ok(result);

        }

        [HttpGet("GetBrands")]
        public async Task<ActionResult<List<ProductBrand>>> GetBrands()
        {

            var result = await _productService.GetProductBrandTest();
            return Ok(result);

        }

        [HttpPost("AddProduct")]
        public async Task<ActionResult> AddProduct(ProductDTO product)
        {

            await _productService.CreateProduct(product);
            return Ok("successfully created");

        }

        [HttpDelete("DeleteProduct")]
        public async Task<ActionResult> DeleteProduct(ProductDTO product)
        {
            await _productService.DeleteProductByName(product);
            return Ok("successfully deleted");
        }
    }
}
