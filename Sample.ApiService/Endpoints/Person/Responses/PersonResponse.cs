namespace Sample.ApiService.Endpoints.Person.Responses;

public record PersonResponse(Guid PersonId, string FirstName, string LastName, DateTime DateOfBirth)
{
    public string FullName => $"{FirstName} {LastName}";
}