using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Core.Enums;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;
using NazarMahal.Core.ActionResponses;

namespace NazarMahal.Application.Interfaces
{
    public interface IOrderService
    {

        Task<ActionResponse<OrderDto>> CreateOrder(CreateOrderRequestDto orderRequest);
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetAllOrders();
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetOpenOrders();
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetInProgressOrder();
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetReadyForPickupOrders();
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetCompletedOrders();
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetCancelledOrders();
        Task<ActionResponse<OrderResponseDto>> GetOrderByOrderId(int orderId);
        Task<ActionResponse<OrderResponseDto>> CancelOrder(int orderId, bool OrderStatus);
        Task<ActionResponse<OrderResponseDto>> UpdateOrderStatus(int orderId, OrderStatus newStatus);
        Task<ActionResponse<IEnumerable<OrderResponseDto>>> GetOrderByUserId(int userId);

    }
}
