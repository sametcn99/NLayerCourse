using App.Repositories.Interfaces;

namespace App.Services.Products;

internal class ProductService(IProductRepository productRepository) : IProductService
{

}
