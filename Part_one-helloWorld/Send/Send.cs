﻿using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory{ HostName = "localhost"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

 channel.QueueDeclare(
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null );

const string message = "Hello world!";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(
    exchange: string.Empty,
    routingKey: "hello",
    basicProperties: null,
    body: body);

Console.WriteLine($" [X] Sent {message}");

Console.WriteLine(" Presse [enter] to exit.");
Console.ReadLine();