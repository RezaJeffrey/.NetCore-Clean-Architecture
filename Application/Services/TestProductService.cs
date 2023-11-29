using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using CoreLayer.Services;
using Utils.Mappings;
using Utils.Exceptions;

namespace Application.Services
{
    //public class TestProductService 
    //{
    //    public CoreService<Product, ProductDTO> CoreService { get; set; }
    //    public TestProductService(CoreService<Product, ProductDTO> coreService)
    //    {
    //        CoreService = coreService;
    //    } 
            
    //    public async Task<List<Product>> GetProductsTest()
    //    {
    //        var products = await CoreService.Table().ToListAsync();
    //        return products;
    //    }

    //    public async Task<Product> GetProductTest(long Id)
    //    {
    //        var product = await CoreService.FindByIdAsync(Id);
    //        if (product == null) throw new AppRuleException("no such Item in database or you don't have sufficient permissions"); 
    //        return product;
    //    }

    //    public async Task<List<ProductBrand>> GetProductBrandTest()
    //    {
    //        var brands = await CoreService.Table<ProductBrand>().ToListAsync();
    //        return brands;
    //    }

    //    public async Task CreateProduct(ProductDTO input)
    //    {
    //        var product = ObjectMapper.ConvertObject<ProductDTO, Product>(input);
    //        product.BrandId = 3;
    //        product.ProductTypeId = 3;


    //        await CoreService.Create(product, false);
    //        await CoreService.CommitAsync();
    //    }

    //    public async Task DeleteProductByName(ProductDTO input)
    //    {

    //        var deleted = await CoreService.Table()
    //            .Where(i => i.Name == input.Name)
    //            .FirstOrDefaultAsync();

    //        if (deleted == null) throw new AppRuleException("Item does not exist in DataBase or you don't have sufficient permissions");

    //        await CoreService.Delete(deleted.Id, false);
    //        await CoreService.CommitAsync();

    //    }
    //}
}
