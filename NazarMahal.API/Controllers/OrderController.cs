using NazarMahal.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Core.Enums;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;
using NazarMahal.Core.ActionResponses;

namespace NazarMahal.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<OrderDto>>> CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            var response = await _orderService.CreateOrder(request);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Get orders with optional filters
        /// Query params:
        /// - status: filter by order status (Open, Completed, Cancelled)
        /// - userId: filter by user ID
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OrderResponseDto>>>> GetOrders(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] int? userId = null)
        {
            // Get orders by status
            if (status.HasValue)
            {
                var response = status.Value switch
                {
                    OrderStatus.New => await _orderService.GetOpenOrders(),
                    OrderStatus.ReadyForPickup => await _orderService.GetOpenOrders(),
                    OrderStatus.Completed => await _orderService.GetCompletedOrders(),
                    OrderStatus.Cancelled => await _orderService.GetCancelledOrders(),
                    _ => await _orderService.GetAllOrders()
                };
                return response.ToApiResponse();
            }

            // Get orders by user ID
            if (userId.HasValue)
            {
                var response = await _orderService.GetOrderByUserId(userId.Value);
                return response.ToApiResponse();
            }

            // Get all orders
            var allResponse = await _orderService.GetAllOrders();
            return allResponse.ToApiResponse();
        }

        /// <summary>
        /// Get a specific order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<OrderResponseDto>>> GetOrder(int id)
        {
            var response = await _orderService.GetOrderByOrderId(id);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Update order status
        /// Merged endpoint: replaces UpdateOrderStatus, CancelOrder, UncancelOrder
        /// Body:
        /// {
        ///   "status": "Cancelled" | "ReadyForPickup" | "Completed"
        /// }
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponseDto<OrderResponseDto>>> UpdateOrder(
            int id,
            [FromBody] UpdateOrderStatusRequestDto request)
        {
            var response = await _orderService.UpdateOrderStatus(id, request.Status);
            return response.ToApiResponse();
        }
    }
}
