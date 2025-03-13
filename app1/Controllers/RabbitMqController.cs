using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace app1.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RabbitMqController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Publish(string message)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "myuser", Password = "1234!" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "myqueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "myqueue", body: body);
        Console.WriteLine($" [x] Sent {message}");

        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> InitListner()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "myuser", Password = "1234!" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "myqueue", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync("myqueue", autoAck: true, consumer: consumer);

        Console.Read();

        return Ok("Waiting for messages") ;
    }
}
