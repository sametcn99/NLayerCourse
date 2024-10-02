using App.Repositories.Interfaces;
using App.Repositories.Products;
using System.Net;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);

        var productDtos = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productDtos);
    }

    public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            ServiceResult<Product>.Failure(new List<string> { "Product not found" }, HttpStatusCode.NotFound);
        }

        var productDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);

        return ServiceResult<ProductDto>.Success(productDto);
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await productRepository.AddAsync(product);

        await unitOfWork.SaveChangesAsync();

        return ServiceResult<CreateProductResponse>.Success(new CreateProductResponse(product.Id));
    }

    public async Task<ServiceResult> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return ServiceResult.Failure(new List<string> { "Product not found" }, HttpStatusCode.NotFound);
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        productRepository.Update(product);

        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteProductAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return ServiceResult.Failure(new List<string> { "Product not found" }, HttpStatusCode.NotFound);
        }

        productRepository.Delete(product);

        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success();
    }
}
