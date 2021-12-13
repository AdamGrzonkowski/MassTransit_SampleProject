using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contracts;

namespace Twitch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;

        public OrderController(ILogger<OrderController> logger, IRequestClient<SubmitOrder> submitOrderRequestClient)
        {
            _logger = logger;
            _submitOrderRequestClient = submitOrderRequestClient;
        }

        [HttpPost]
        [ProducesResponseType(((int)HttpStatusCode.Accepted), Type = typeof(OrderSubmissionAccepted))]
        [ProducesResponseType(((int)HttpStatusCode.BadRequest), Type = typeof(OrderSubmissionRejected))]
        public async Task<IActionResult> Post(string id, string customerNumber)
        {
            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new SubmitOrder
            {
                OrderId = id,
                Timestamp = DateTimeOffset.UtcNow,
                CustomerNumber = customerNumber
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Accepted(response.Message);
            }
            else
            {
                var response = await rejected;
                return BadRequest(response.Message);
            }
        }
    }
}