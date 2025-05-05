using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

Console.WriteLine("Please specify a chat room: ");
var chatRoomName = Console.ReadLine();

var exchangeName = "chat2";
var queueName = Guid.NewGuid().ToString();

await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
await channel.QueueDeclareAsync(queueName, true, true, true);
await channel.QueueBindAsync(queueName, exchangeName, chatRoomName);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var text = System.Text.Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine(text);
};

await channel.BasicConsumeAsync(queueName, true, consumer);

var input = Console.ReadLine();
while (input != null)
{
    var bytes = System.Text.Encoding.UTF8.GetBytes(input);
    var props = new BasicProperties();
    await channel.BasicPublishAsync(exchangeName, chatRoomName, false, props, bytes);
    input = Console.ReadLine();
}

await channel.CloseAsync();
await connection.CloseAsync();
