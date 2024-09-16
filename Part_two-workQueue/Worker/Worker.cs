using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory{ HostName = "localhost"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

 channel.QueueDeclare(
    queue: "task_queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null );

//This tell to rbMQ to not sent another data until the previews one has ben processed 
 channel.BasicQos(prefetchSize:0, prefetchCount: 1, global: false);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) => 
{
    var body = ea.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [X] Received {message}");

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);

    System.Console.WriteLine(" [X] Done.");

    // It tells the rbMQ that the work has been finished and the data can be deleted
    // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
channel.BasicConsume(
    queue: "task_queue",
    autoAck: true,
    consumer: consumer
);

Console.WriteLine(" Press [ enter ] to exit.");
Console.ReadLine();