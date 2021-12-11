using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contracts.Dtos;
using Sample.Contracts.Events;
using Sample.Contracts.Requests;
using Sample.Contracts.Responses;

namespace Twitch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
        private readonly IRequestClient<OrderStatus> _getOrderStatusRequestClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(
            ILogger<OrderController> logger,
            IRequestClient<SubmitOrder> submitOrderRequestClient,
            IRequestClient<OrderStatus> getOrderStatusRequestClient,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
            _getOrderStatusRequestClient = getOrderStatusRequestClient;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [ProducesResponseType(((int)HttpStatusCode.OK), Type = typeof(GetOrderStatusResponse))]
        [ProducesResponseType(((int)HttpStatusCode.NotFound))]
        public async Task<IActionResult> GetOrderStatus(Guid orderId)
        {
            var (status, notFound) = await _getOrderStatusRequestClient.GetResponse<OrderStatus, OrderNotFound>(new
            {
                OrderId = orderId
            });

            if (status.IsCompletedSuccessfully)
            {
                var response = await status;
                return Ok(new GetOrderStatusResponse
                {
                    OrderId = response.Message.OrderId,
                    State = response.Message.State
                });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(((int)HttpStatusCode.Accepted), Type = typeof(OrderSubmissionAcceptedResponse))]
        [ProducesResponseType(((int)HttpStatusCode.BadRequest), Type = typeof(OrderSubmissionRejectedResponse))]
        public async Task<IActionResult> SubmitOrder(SubmitOrderRequest request)
        {
            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = request.OrderId,
                Timestamp = DateTimeOffset.UtcNow,
                CustomerNumber = request.CustomerNumber
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Accepted(new OrderSubmissionAcceptedResponse
                {
                    OrderId = response.Message.OrderId,
                    Timestamp = response.Message.Timestamp,
                    CustomerNumber = response.Message.CustomerNumber
                });
            }
            else
            {
                var response = await rejected;
                return BadRequest(new OrderSubmissionRejectedResponse
                {
                    OrderId = response.Message.OrderId,
                    Timestamp = response.Message.Timestamp,
                    CustomerNumber = response.Message.CustomerNumber,
                    Reason = response.Message.Reason
                });
            }
        }

        [HttpPatch]
        [ProducesResponseType(((int)HttpStatusCode.Accepted))]
        public async Task<IActionResult> AcceptOrder(Guid orderId)
        {
            await _publishEndpoint.Publish(new OrderAccepted
            {
                OrderId = orderId,
                Timestamp = DateTimeOffset.UtcNow
            });

            return Accepted();
        }
    }
}