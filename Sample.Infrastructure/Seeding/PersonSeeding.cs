using Bogus;
using Microsoft.EntityFrameworkCore;
using Person=Sample.Infrastructure.Entities.Person;

namespace Sample.Infrastructure.Seeding;

public static class PersonSeeding
{
    private static readonly Faker<Person> PersonFaker = GetPersonFaker();
    
    private static Faker<Person> GetPersonFaker() => new Faker<Person>()
        .UseSeed(42069)
        .RuleFor(x => x.FirstName, f => f.Person.FirstName)
        .RuleFor(x => x.LastName, f => f.Person.LastName)
        .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth.ToUniversalTime());

    public static async Task SeedAsync(DbContext context, CancellationToken ct)
    {
        var peopleSet = context.Set<Person>();
        if (await peopleSet.AnyAsync(ct))
            return;
        
        var people = PersonFaker.Generate(69);
        await peopleSet.AddRangeAsync(people, ct);

        await context.SaveChangesAsync(ct);
    }
}