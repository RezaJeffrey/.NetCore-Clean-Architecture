using Domain.CoreServices;
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
    }
}
