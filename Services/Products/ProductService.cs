using App.Repositories.Interfaces;
using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);

        var productDtos = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productDtos);
    }

    public async Task<ServiceResult<List<ProductDto?>>> GetAllListAsync()
    {
        var products = await productRepository.GetAll().ToListAsync();

        #region Manual Mapping
        //var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();
        #endregion

        var productsAsDto = mapper.Map<List<ProductDto>>(products); // Using AutoMapper to map the entities to DTOs

        return ServiceResult<List<ProductDto>>.Success(productsAsDto)!;
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAll()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productsAsDto)!;
    }

    public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return ServiceResult<ProductDto?>.Failure(new List<string> { "Product not found" }, HttpStatusCode.NotFound);
        }

        #region Manual Mapping
        //var productDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
        #endregion

        var productDto = mapper.Map<ProductDto>(product); // Using AutoMapper to map the entity to DTO

        return ServiceResult<ProductDto>.Success(productDto!)!;
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock
        };

        await productRepository.AddAsync(product);

        await unitOfWork.SaveChangesAsync();

        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
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

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest updateProductStockRequest)
    {
        var product = await productRepository.GetByIdAsync(updateProductStockRequest.ProductId);

        if (product == null) return ServiceResult.Failure(new List<string> { "Product not found" }, HttpStatusCode.NotFound);

        product.Stock = updateProductStockRequest.Quantity;

        productRepository.Update(product);

        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product == null)
        {
            return ServiceResult.Failure(new List<string> { "Product not found" }, HttpStatusCode.NotFound);
        }

        productRepository.Delete(product);

        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
