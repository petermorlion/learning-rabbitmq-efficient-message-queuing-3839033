using System.Security.Authentication;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://backoffice:backoffice@localhost:5672");
var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync("DLX", ExchangeType.Direct, true);
await channel.QueueDeclareAsync("deadletters", true, false, false);
await channel.QueueBindAsync("deadletters", "DLX", "");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var msg = System.Text.Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    var deathReasonBytes = eventArgs.BasicProperties.Headers["x-first-death-reason"] as byte[];
    var deathReason = System.Text.Encoding.UTF8.GetString(deathReasonBytes);
    Console.WriteLine($"Deadletter: {msg}. Reason: {deathReason}");
};

await channel.BasicConsumeAsync("deadletters", true, consumer);

Console.ReadLine();

await channel.CloseAsync();
await connection.CloseAsync();
