using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync("backOfficeQueue", true, false, false);
var headers = new Dictionary<string, object>
{
    { "subject", "tour" },
    { "action", "booked" },
    { "x-match", "any" }
};

await channel.QueueBindAsync("backOfficeQueue", "webappExchange", "", headers);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var msg = System.Text.Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    var subject = System.Text.Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["subject"] as byte[]);
    var action = System.Text.Encoding.UTF8.GetString(eventArgs.BasicProperties.Headers["action"] as byte[]);
    Console.WriteLine($"{subject} {action} : ${msg}");
};

await channel.BasicConsumeAsync("backOfficeQueue", true, consumer);

Console.ReadLine();

await channel.CloseAsync();
await connection.CloseAsync();

