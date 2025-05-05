using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync("backOfficeQueue", true, false, false);
await channel.QueueBindAsync("backOfficeQueue", "webappExchange", "tour.*");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var msg = System.Text.Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine($"{eventArgs.RoutingKey} : ${msg}");
};

await channel.BasicConsumeAsync("backOfficeQueue", true, consumer);

Console.ReadLine();

await channel.CloseAsync();
await connection.CloseAsync();

