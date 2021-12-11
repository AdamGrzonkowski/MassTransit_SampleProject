using MassTransit;
using Sample.Contracts.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// adds MassTransit' Mediator
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddRequestClient<SubmitOrder>();
    cfg.AddRequestClient<OrderStatus>();

    cfg.UsingRabbitMq(); // add RabbitMQ as broker for messages
});

builder.Services.AddMassTransitHostedService(); // required to use RabbitMQ (for starting/stopping bus along with HealthChecks configuration)

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();