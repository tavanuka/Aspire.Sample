using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Sample.ApiService.Endpoints.Person.Responses;
using Sample.Infrastructure;

namespace Sample.ApiService.Endpoints.Person.Get;

public class GetPeopleEndpoint : Ep.NoReq.Res<HashSet<PersonResponse>>
{
    private readonly CoreDbContext _dbContext;
    public GetPeopleEndpoint(CoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get("/api/person/list");
        AllowAnonymous();
        
        Description(x => x.AutoTagOverride("Person"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var people = await _dbContext.People
            .AsNoTracking()
            .Select(x => new PersonResponse(x.Id, x.FirstName, x.LastName, x.DateOfBirth))
            .ToHashSetAsync(ct);

        await Send.OkAsync(people, ct);
    }
}