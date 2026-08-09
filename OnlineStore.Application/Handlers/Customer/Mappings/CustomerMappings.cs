using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Customer.Models;

namespace OnlineStore.Application.Handlers.Customer.Mappings
{
    public static class CustomerMappings
    {
        public static CustomerDto ToDto(this CustomerDetails customerDetails)
        {
            return new CustomerDto
            {
                Address = customerDetails.Customer.Address,
                Email = customerDetails.Customer.Email.Value,
                Id = customerDetails.Customer.Id,
                Phone = customerDetails.Customer.Phone,
                userSummaryDto = new UserSummaryDto
                {
                    Id = customerDetails.UserId,
                    Name = customerDetails.Name,
                    Username = customerDetails.Username,
                    IsActive = customerDetails.IsActive
                }
            };
        }
    }
}
