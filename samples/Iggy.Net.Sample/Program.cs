using System.Text;
using System.Text.Json;
using Iggy.Net.Contracts;
using Iggy.Net.DependencyInjection;
using Iggy.Net.Kinds;
using Iggy.Net.Messages;
using Iggy.Net.Stream;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIggy(options =>
{
    options.AddStream(streamOptions =>
    {
        streamOptions.StreamId = 3;
        streamOptions.AddTopic(topicOptions =>
        {

        });
        streamOptions.AddTopic(topicOptions =>
        {

        });
    });
});

var app = builder.Build();

app.UseIggy();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", async ([FromServices] IMessageStream stream) =>
{
    await stream.SendMessagesAsync(1, 1, 
        new MessageSendRequest()
        {
            Messages = new List<Message>(){ new Message()
            {
                Id = Guid.NewGuid(),
                Payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    Test = "Test"
                }))
            }},
            Partitioning = Partitioning.PartitionId(1)
        });
    return Results.Ok();
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();