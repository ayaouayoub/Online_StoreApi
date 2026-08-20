using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Handlers.Review.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Application.Exceptions;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class UpdateReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUser _currentUser;

        public UpdateReviewHandler(IReviewRepository reviewRepository, ICurrentUser currentUser, ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _currentUser = currentUser;
            _customerRepository = customerRepository;
        }

        public async Task ExecuteAsync(UpdateReviewCommand command)
        {
            var result = await _customerRepository.GetByUserIdAsync(_currentUser.UserId) ?? throw new ForbiddenException("Customer profile not found.");

            var review = await _reviewRepository.GetByIdAsync(command.ReviewId) ?? throw new NotFoundException("Review not found.");
            
            if (review.CustomerId != result.Customer.Id) throw new ForbiddenException("You can only update your own reviews.");

            review.Update(command.Rating, command.ReviewText);

            await _reviewRepository.UpdateAsync(review);
        }
    }
}
