using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Review.Mappings;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class CreateReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUser _currentUser;

        public CreateReviewHandler(IReviewRepository reviewRepository, IProductRepository productRepository, ICustomerRepository customerRepository, ICurrentUser currentUser)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _currentUser = currentUser;
        }

        public async Task<ReviewDto> ExecuteAsync(CreateReviewCommand command)
        {
            var customerDetails = await _customerRepository.GetByIdAsync(_currentUser.UserId) ?? throw new ForbiddenException("Customer not found."); 

            var product = await _productRepository.GetByIdAsync(command.ProductId) ?? throw new DomainException("Product not found.");

            var review = Domain.Entities.Review.Create(product, customerDetails.Customer, command.Rating, command.ReviewText);

            return (await _reviewRepository.CreateAsync(review)).ToDto();
        }
    }
}