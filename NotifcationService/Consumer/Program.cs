using Microsoft.AspNetCore.Http.Connections;
using System.Reflection;
using Consumer;
using Consumer.Commands.Handlers;
using Consumer.Contexts;
using Consumer.Hubs;
using Consumer.Models;
using Consumer.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IRequestHandler<DebeziumMessage>, NotificationHandler>();
builder.Services.AddHostedService<NotificationConsumer>();

builder.Services.AddSingleton<ConnectionFactory>(serviceProvider =>
{
    var uri = new Uri("amqp://guest:guest@rabbit:5674/");
    return new ConnectionFactory
    {
        Uri = uri,
        DispatchConsumersAsync = true,
        HostName = "localhost",
        VirtualHost = "/"
    };
});

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(Common.Global.Constants.connString));

builder.Services.AddScoped<NotificationHub>();
builder.Services.AddSignalR();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("reactApp", builder =>
    {
        builder.WithOrigins("http:localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.UseCors("reactApp");

app.MapHub<NotificationHub>("/notification", options =>
{
    options.Transports = HttpTransportType.WebSockets;
});

app.Run();