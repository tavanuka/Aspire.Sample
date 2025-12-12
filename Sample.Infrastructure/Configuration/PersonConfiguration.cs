using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Infrastructure.Entities;

namespace Sample.Infrastructure.Configuration;

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
	public void Configure(EntityTypeBuilder<Person> builder)
	{
		builder.ToTable("People");
		builder.HasKey(x => x.Id);
	}
}