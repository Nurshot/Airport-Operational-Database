using System.Text.RegularExpressions;
using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Models;
using AODB.Application.Flights.Commands.CreateFlight;
using AODB.Application.Flights.Commands.DeleteFlight;
using AODB.Application.Flights.Commands.UpdateFlight;
using AODB.Application.Flights.Commands.UpdateFlightTimes;
using AODB.Application.Flights.Queries.GetFlightById;

using AODB.Application.Flights.Queries.GetFlights;
using AODB.Domain.Constants;  // Roles sabitleri için
using Microsoft.AspNetCore.Authorization;  // Authorize attribute için


namespace AODB.Web.Endpoints;

public class Flights : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this)
        .RequireAuthorization();
        group.MapGet("/", GetFlights);

        group.MapGet("/{id}", GetFlightById);

        group.MapPost("/", CreateFlight)
            .RequireAuthorization("admin");
        group.MapPut("/{id}", UpdateFlight)
            .RequireAuthorization("admin");
        group.MapDelete("/{id}", DeleteFlight)
            .RequireAuthorization("admin");

        group.MapPut("/times", UpdateFlightTimes)
            .RequireAuthorization("admin");
        //.MapPut(UpdateTodoItem, "{id}")
        //.MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
        //.MapDelete(DeleteTodoItem, "{id}");
    }


    public Task<int> CreateFlight(ISender sender, CreateFlightCommand command)
    {
        return sender.Send(command);
        //return Task.FromResult(0);
    }

    public async Task<IResult> UpdateFlight(ISender sender, int id, UpdateFlightCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();  //HTTP KODU 204 Güncellendi.
    }
    public async Task<List<FlightDto>> GetFlights(ISender sender)
    {
        return await sender.Send(new GetFlightsQuery());
    }

    public async Task<FlightDto> GetFlightById(ISender sender, int id)
    {
        return await sender.Send(new GetFlightByIdQuery(id));
       
    }

    public async Task<IResult> DeleteFlight(ISender sender, int id)
    {
        await sender.Send(new DeleteFlightCommand(id));
        return Results.NoContent();
    }

    public async Task<IResult> UpdateFlightTimes(ISender sender, UpdateFlightTimesCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }
}
