using RabbitMQ.Client;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

// declare resources here, handle consumed events, etc

await channel.CloseAsync();
await connection.CloseAsync();
