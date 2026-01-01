using NazarMahal.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Core.Enums;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;

namespace NazarMahal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponseDto<OrderDto>>> CreateOrder(CreateOrderRequestDto orderRequest)
        {
            var response = await _orderService.CreateOrder(orderRequest);
            return response.ToApiResponse();
        }

        [HttpGet("GetOrderByOrderId")]
        public async Task<ActionResult<ApiResponseDto<OrderResponseDto>>> GetOrderByOrderId(int orderId)
        {
            var response = await _orderService.GetOrderByOrderId(orderId);
            return response.ToApiResponse();
        }

        [HttpGet("GetAllOrdersByUserId")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OrderResponseDto>>>> GetOrderByUserId(int userId)
        {
            var response = await _orderService.GetOrderByUserId(userId);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OrderResponseDto>>>> GetAllOrders()
        {
            var response = await _orderService.GetAllOrders();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetOpenOrders")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OrderResponseDto>>>> GetOpenOrders()
        {
            var response = await _orderService.GetOpenOrders();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetCompletedOrders")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OrderResponseDto>>>> GetCompletedOrders()
        {
            var response = await _orderService.GetCompletedOrders();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetCancelledOrders")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<OrderResponseDto>>>> GetCancelledOrders()
        {
            var response = await _orderService.GetCancelledOrders();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<ActionResult<ApiResponseDto<OrderResponseDto>>> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, newStatus);
            return response.ToApiResponse();
        }
        
        [HttpPut("CancelOrder/{orderId}")]
        public async Task<ActionResult<ApiResponseDto<OrderResponseDto>>> CancelOrder(int orderId)
        {
            var response = await _orderService.CancelOrder(orderId, true);
            return response.ToApiResponse();
        }
        
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPut("UncancelOrder/{orderId}")]
        public async Task<ActionResult<ApiResponseDto<OrderResponseDto>>> UncancelOrder(int orderId)
        {
            var response = await _orderService.CancelOrder(orderId, false);
            return response.ToApiResponse();
        }



    }
}
