using Library.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.Mapping
{
    public class ReserveMapping : IEntityTypeConfiguration<Reserve>
    {
        public void Configure(EntityTypeBuilder<Reserve> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Returned)
               .IsRequired()
               .HasColumnType("int");

            builder.ToTable("Reserves");
        }
    }
}
