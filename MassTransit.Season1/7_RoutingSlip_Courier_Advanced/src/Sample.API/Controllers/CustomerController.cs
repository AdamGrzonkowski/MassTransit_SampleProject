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
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CustomerController(
            ILogger<OrderController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> CloseCustomerAccount(Guid customerId, string customerNumber)
        {
            await _publishEndpoint.Publish<CustomerAccountClosed>(new
            {
                CustomerId = customerId,
                CustomerNumber = customerNumber
            });

            return Ok();
        }
    }
}