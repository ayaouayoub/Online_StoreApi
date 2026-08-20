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
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Handlers.Review
{
    public sealed class DeleteMyReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICustomerRepository _customerRepository;

        public DeleteMyReviewHandler(IReviewRepository reviewRepository, ICurrentUser currentUser, ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _currentUser = currentUser;
            _customerRepository = customerRepository;
        }

        public async Task ExecuteAsync(DeleteMyReviewCommand command)
        {
            var customerDetails = await _customerRepository.GetByUserIdAsync(_currentUser.UserId) ?? throw new ForbiddenException("Customer profile not found.");

            var review = await _reviewRepository.GetByIdAsync(command.ReviewId) ?? throw new DomainException("Review not found.");

            if (review.CustomerId != customerDetails.Customer.Id) throw new ForbiddenException("You can only delete your own review.");

            review.Delete(_currentUser.User, command.DeleteReason);

            await _reviewRepository.DeleteAsync(review);
        }
    }
}
