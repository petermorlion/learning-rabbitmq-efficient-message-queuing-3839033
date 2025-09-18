using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace ExploreCalifornia.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        [HttpPost]
        [Route("Book")]
        public async Task<IActionResult> Book()
        {
            var tourname = Request.Form["tourname"];
            var name = Request.Form["name"];
            var email= Request.Form["email"];
            var needsTransport = Request.Form["transport"] == "on";

            var message = $"{tourname};{name};{email}";
            var headers = new Dictionary<string, object>
            {
                { "subject", "tour" },
                { "action", "booked" }
            };

            await SendMessage(headers, message);

            if (needsTransport)
            {
                var needsTransportHeaders = new Dictionary<string, object>
                {
                    { "subject", "transport" },
                    { "action", "booked" }
                };

                SendMessage(needsTransportHeaders, message);
            }

            return Redirect($"/BookingConfirmed?tourname={tourname}&name={name}&email={email}");
        }

        [HttpPost]
        [Route("Cancel")]
        public async Task<IActionResult> Cancel()
        {
            var tourname = Request.Form["tourname"];
            var name = Request.Form["name"];
            var email = Request.Form["email"];
            var cancelReason = Request.Form["reason"];

            var message = $"{tourname};{name};{email};{cancelReason}";
            var headers = new Dictionary<string, object>
            {
                { "subject", "tour" },
                { "action", "canceled" }
            };

            await SendMessage(headers, message);

            return Redirect($"/BookingCanceled?tourname={tourname}&name={name}");
        }

        private async Task SendMessage(IDictionary<string, object> headers, string message)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://webapp:webapp@localhost:5672");
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            var bytes = Encoding.UTF8.GetBytes(message);
            var props = new BasicProperties();
            props.Headers = headers;
            props.UserId = "webapp";
            await channel.BasicPublishAsync("webappExchange", "", false, props, bytes);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
