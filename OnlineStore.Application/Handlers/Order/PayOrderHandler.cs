using OnlineStore.Application.Contracts.Payments;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Order.Commands;
using OnlineStore.Application.Handlers.Order.Mappings;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces.Services.Payments;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Domain.ValueObjs;

namespace OnlineStore.Application.Handlers.Order;

public sealed class PayOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IPaymentGatewayFactory _paymentGatewayFactory;

    public PayOrderHandler(IOrderRepository orderRepository, ICurrentUser currentUser, ICustomerRepository customerRepository, IPaymentMethodRepository paymentMethodRepository, IPaymentGatewayFactory paymentGatewayFactory)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
        _customerRepository = customerRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _paymentGatewayFactory = paymentGatewayFactory;
    }

    public async Task<OrderDto> ExecuteAsync(PayOrderCommand command)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId) ?? throw new NotFoundException("Order not found.");

        var details = await _customerRepository.GetByUserIdAsync(_currentUser.UserId) ?? throw new ForbiddenException("Customer profile not found.");

        if (order.CustomerId != details.Customer.Id) throw new ForbiddenException("You cannot pay for this order.");

        if (order.Status != OrderStatus.PendingPayment) throw new DomainException("Only pending orders can be paid.");

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId) ?? throw new NotFoundException("Payment method not found.");

        if (!Enum.TryParse<PaymentProvider>(paymentMethod.Name, ignoreCase: true, out var paymentMethodProvider))
        {
            throw new DomainException("Invalid payment method configuration.");
        }

        if (paymentMethodProvider != command.Provider)
        {
            throw new DomainException("The selected payment method does not match the payment provider.");
        }

        var payment = Domain.Entities.Payment.Create
        (
            orderId: order.Id,
            money: new Money(order.TotalAmount, new Currency("USD")),
            paymentMethod: paymentMethod
        );

        var gateway = _paymentGatewayFactory.Get(command.Provider);

        var paymentResult = await gateway.PayAsync(new PaymentRequest
        {
            OrderId = order.Id,
            Amount = payment.Money.Amount,
            Currency = payment.Money.Currency.Code
        });

        if (!paymentResult.Success)
        {
            payment.MarkAsFailed();

            await _orderRepository.CreateFailedPaymentAndCancelOrderAsync(payment, order.Id);

            throw new DomainException(paymentResult.ErrorMessage ?? "Payment failed.");
        }

        payment.MarkAsSucceeded(paymentResult.TransactionId!);

        order.MarkAsPaid();

        await _orderRepository.CreatePaymentAndMarkAsPaidAsync(payment, order);

        return order.ToDto();
    }
}