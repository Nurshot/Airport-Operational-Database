using AODB.Application.Aircrafts.Commands.CreateAircraft;
using AODB.Application.Aircrafts.Commands.DeleteAircraft;
using AODB.Application.Aircrafts.Commands.UpdateAircraft;
using AODB.Application.Aircrafts.Queries.GetAircraftById;
using AODB.Application.Aircrafts.Queries.GetAircrafts;
using AODB.Application.Common.Interfaces;


namespace AODB.Web.Endpoints;

public class Aircrafts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this)
        .RequireAuthorization(); 
        group.MapGet("/", GetAircrafts);

        group.MapGet("/{id}", GetAircraftById);


        group.MapPost("/", CreateAircraft)
            .RequireAuthorization("admin");
        group.MapPut("/{id}", UpdateAircraft)
           .RequireAuthorization("admin");
        group.MapDelete("/{id}", DeleteAircraft)
            .RequireAuthorization("admin");
    }


    public Task<int> CreateAircraft(ISender sender, CreateAircraftCommand command, IUser currentUser)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateAircraft(ISender sender, int id, UpdateAircraftCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();  
    }
    public async Task<List<AircraftDto>> GetAircrafts(ISender sender)
    {
        return await sender.Send(new GetAircraftsQuery());
    }

    public async Task<AircraftDto> GetAircraftById(ISender sender, int id)
    {
        return await sender.Send(new GetAircraftByIdQuery(id));
    }


    public async Task<IResult> DeleteAircraft(ISender sender, int id)
    {
        await sender.Send(new DeleteAircraftCommand(id));
        return Results.NoContent();
    }



}
