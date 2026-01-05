using NazarMahal.Application.DTOs.GlassesDto;
using NazarMahal.Application.DTOs.OrderDto;
using NazarMahal.Application.RequestDto.OrderRequestDto;
using NazarMahal.Application.ResponseDto.OrderResponseDto;
using NazarMahal.Core.Entities;

namespace NazarMahal.Application.Mappers
{
    public static class OrderMappingExtensions
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            if (order == null) return null;

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId.ToString(),
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                OrderCreatedDate = order.OrderCreatedDate,
                OrderCreatedTime = order.OrderCreatedTime,
                PhoneNumber = order.PhoneNumber,
                UserEmail = order.UserEmail,
                OrderNumber = order.OrderNumber,
                FirstName = order.FirstName,
                LastName = order.LastName,
                PaymentMethod = order.PaymentMethod,
                OrderItems = order.OrderItems?.Select(oi => oi.ToOrderItemDto()).ToList() ?? new List<OrderItemDto>()
            };
        }

        /// <summary>
        /// Map Order to OrderResponseDto
        /// </summary>
        public static OrderResponseDto ToOrderResponseDto(this Order order)
        {
            if (order == null) return null;

            var orderDate = order.OrderCreatedDate.ToDateTime(order.OrderCreatedTime);

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId.ToString(),
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus.ToString(),
                OrderDate = orderDate,
                PhoneNumber = order.PhoneNumber,
                UserEmail = order.UserEmail,
                OrderNumber = order.OrderNumber,
                FirstName = order.FirstName,
                LastName = order.LastName,
                PaymentMethod = order.PaymentMethod,
                OrderItems = order.OrderItems?.Select(oi => oi.ToOrderItemDto()).ToList() ?? new List<OrderItemDto>()
            };
        }

        /// <summary>
        /// Map OrderItem to OrderItemDto
        /// </summary>
        public static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        {
            if (orderItem == null) return null;

            return new OrderItemDto
            {
                OrderItemId = orderItem.OrderItemId,
                GlassesId = orderItem.GlassesId,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalAmount = orderItem.TotalAmount,
                GlassesName = orderItem.Glasses?.Name,
                GlassesDetails = orderItem.Glasses != null ? new GlassesDetailsDto
                {
                    GlassesId = orderItem.Glasses.Id,
                    GlassesName = orderItem.Glasses.Name,
                    Description = orderItem.Glasses.Description,
                    Price = orderItem.Glasses.Price,
                    Brand = orderItem.Glasses.Brand,
                    Model = orderItem.Glasses.Model,
                    FrameType = orderItem.Glasses.FrameType,
                    LensType = orderItem.Glasses.LensType,
                    Color = orderItem.Glasses.Color,
                    Attachments =  orderItem.Glasses.Attachments?.Select(a => new GlassesAttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        StoragePath = a.StoragePath,
                    }).ToList()
                } : null
            };
        }

        /// <summary>
        /// Map IEnumerable of Order to IEnumerable of OrderResponseDto
        /// </summary>
        public static IEnumerable<OrderResponseDto> ToOrderResponseDtoList(this IEnumerable<Order> orders)
        {
            if (orders == null) return Enumerable.Empty<OrderResponseDto>();
            return orders.Select(o => o.ToOrderResponseDto());
        }

        /// <summary>
        /// Map IEnumerable of OrderItem to List of OrderItemDto
        /// </summary>
        public static List<OrderItemDto> ToOrderItemDtoList(this IEnumerable<OrderItem> items)
        {
            if (items == null) return new List<OrderItemDto>();
            return items.Select(i => i.ToOrderItemDto()).ToList();
        }

        /// <summary>
        /// Map OrderItemRequestDto to OrderItem
        /// </summary>
        public static OrderItem ToOrderItem(this OrderItemRequestDto request)
        {
            if (request == null) return null;

            return new OrderItem
            {
                GlassesId = request.GlassesId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice
            };
        }

        /// <summary>
        /// Map IEnumerable of OrderItemRequestDto to List of OrderItem
        /// </summary>
        public static List<OrderItem> ToOrderItemList(this IEnumerable<OrderItemRequestDto> requests)
        {
            if (requests == null) return new List<OrderItem>();
            return requests.Select(r => r.ToOrderItem()).ToList();
        }
    }
}
