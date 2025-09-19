using System.Text.RegularExpressions;
using AODB.Application.Airports.Commands.CreateAirport;
using AODB.Application.Airports.Commands.UpdateAirport;
using AODB.Application.Airports.Commands.DeleteAirport;
using AODB.Application.Airports.Queries.GetAirports;
using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Models;
using AODB.Domain.Constants;  // Roles sabitleri için
using Microsoft.AspNetCore.Authorization;  // Authorize attribute için


namespace AODB.Web.Endpoints;

public class Airports : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this)
        .RequireAuthorization(); // Sadece admin rolü olan kullanıcılar erişebilir
        group.MapGet("/", GetAirports);

        group.MapPost("/", CreateAirport)
            .RequireAuthorization("admin");
        group.MapPut("/{id}", UpdateAirport)
            .RequireAuthorization("admin");
        group.MapDelete("/{id}", DeleteAirport)
            .RequireAuthorization("admin");
        //.MapPut(UpdateTodoItem, "{id}")
        //.MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
        //.MapDelete(DeleteTodoItem, "{id}");
    }


    public Task<int> CreateAirport(ISender sender, CreateAirportCommand command)
    {
        return sender.Send(command);
        //return Task.FromResult(0);
    }

    public async Task<IResult> UpdateAirport(ISender sender, int id, UpdateAirportCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();  //HTTP KODU 204 Güncellendi.
    }
    public async Task<List<AirportDto>> GetAirports(ISender sender)
    {
        return await sender.Send(new GetAirportsQuery());
    }

    public async Task<IResult> DeleteAirport(ISender sender, int id)
    {
        await sender.Send(new DeleteAirportCommand(id));
        return Results.NoContent();
    }
}
