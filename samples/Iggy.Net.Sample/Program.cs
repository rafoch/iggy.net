using Iggy.Net.Contracts;
using Iggy.Net.DependencyInjection;
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
        streamOptions.AddTopic(topicOptions =>
        {

        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", ([FromServices] IMessageStream stream) =>
{
    stream.CreateStreamAsync(new StreamRequest()
    {
        Name = "name",
        StreamId = 1
    });
    return Results.Ok();
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();