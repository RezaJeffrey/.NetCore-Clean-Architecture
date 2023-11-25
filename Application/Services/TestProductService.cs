using Domain.CoreServices;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TestProductService 
    {
        public CoreService<Product> CoreService { get; set; }
        public TestProductService(CoreService<Product> coreService)
        {
            CoreService = coreService;
        } 
            
        public async Task<List<Product>> GetProductsTest()
        {
            var products = await CoreService.Table().ToListAsync();
            return products;
        }

        public async Task<Product> GetProductTest(long Id)
        {
            var product = await CoreService.FindByIdAsync(Id);
            return product;
        }

        public async Task<List<ProductBrand>> GetProductBrandTest()
        {
            var brands = await CoreService.Table<ProductBrand>().ToListAsync();
            return brands;
        }

        public async Task CreateProduct(ProductDTO input)
        {
            var product = new Product();
            product.Name = input.Name;
            product.Description = input.Description;    
            product.Price = input.Price;
            product.ImageUrl = input.ImageUrl;
            product.BrandId = null;
            product.ProductTypeId = 3;


            await CoreService.Add(product, false);
            await CoreService.CommitAsync();
        }
    }
}
