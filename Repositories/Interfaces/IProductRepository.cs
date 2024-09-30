using App.Repositories.Products;

namespace App.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    public Task<List<Product>> GetTopPriceProductsAsync(int count);

}
