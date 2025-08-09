using Library.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repository.Data.Config
{
    public class BorrowingConfigration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> builder)
        {
            builder.Property(b=>b.FineAmount)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.HasOne(f=>f.Fine)
                .WithOne(f=>f.Borrowing)
                .HasForeignKey<Fine>(f=>f.BorrowingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
