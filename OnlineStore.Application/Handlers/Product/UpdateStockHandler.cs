using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Product.Commands;
using OnlineStore.Application.Handlers.Product.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Product
{
    public sealed class UpdateStockHandler
    {
        private readonly IProductRepository _productRepository;

        public UpdateStockHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> ExecuteAsync(UpdateStockCommand command)
        {
            if (command.QuantityChange == 0) throw new DomainException("Stock change cannot be zero.");

            Domain.Entities.Product product = await _productRepository.GetByIdAsync(command.ProductId) ?? throw new NotFoundException("Product not found.");

            product.ChangeQuantityInStock(command.QuantityChange);

            await _productRepository.UpdateStockAsync(command.ProductId, command.QuantityChange);

            return product.ToDto();
        }
    }
}
