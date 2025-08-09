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
    public class BookConfigration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b=>b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b=>b.Author)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b=>b.ISBN)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b=>b.CoverImageUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b=>b.Description)
                .IsRequired()
                .HasColumnType("varchar(max)");

            builder.HasMany(b => b.Borrowings)
                .WithOne(b => b.Book)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
