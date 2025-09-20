using AODB.Application.Airlines.Commands.CreateAirline;
using AODB.Application.Airlines.Commands.UpdateAirline;
using AODB.Application.Airlines.Commands.DeleteAirline;
using AODB.Application.Airlines.Queries.GetAirlines;
using AODB.Application.Airlines.Queries.GetAirlineById;
using AODB.Application.Common.Interfaces;


namespace AODB.Web.Endpoints;

public class Airlines : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this)
        .RequireAuthorization(); 
        group.MapGet("/", GetAirlines);

        group.MapGet("/{id}", GetAirlineById);


        group.MapPost("/", CreateAirline)
            .RequireAuthorization("admin");
        group.MapPut("/{id}", UpdateAirline)
           .RequireAuthorization("admin");
        group.MapDelete("/{id}", DeleteAirline)
            .RequireAuthorization("admin");
        //.MapPut(UpdateTodoItem, "{id}")
        //.MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
        //.MapDelete(DeleteTodoItem, "{id}");
    }


    public Task<int> CreateAirline(ISender sender, CreateAirlineCommand command, IUser currentUser)
    {
        return sender.Send(command);
        //return Task.FromResult(0);
    }

    public async Task<IResult> UpdateAirline(ISender sender, int id, UpdateAirlineCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();  //HTTP KODU 204 Güncellendi.
    }
    public async Task<List<AirlineDto>> GetAirlines(ISender sender)
    {
        return await sender.Send(new GetAirlinesQuery());
    }

    public async Task<AirlineDto> GetAirlineById(ISender sender, int id)
    {
         return await sender.Send(new GetAirlineByIdQuery(id));
        
    }

    public async Task<IResult> DeleteAirline(ISender sender, int id)
    {
        await sender.Send(new DeleteAirlineCommand(id));
        return Results.NoContent();
    }



}
