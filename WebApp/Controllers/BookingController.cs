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

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("webappExchange", ExchangeType.Fanout, true);

            var message = $"{tourname};{name};{email}";
            var bytes = Encoding.UTF8.GetBytes(message);
            var props = new BasicProperties();
            await channel.BasicPublishAsync("webappExchange", "", false, props, bytes);

            await channel.CloseAsync();
            await connection.CloseAsync();

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

            // Send cancel message here

            return Redirect($"/BookingCanceled?tourname={tourname}&name={name}");
        }
    }
}
