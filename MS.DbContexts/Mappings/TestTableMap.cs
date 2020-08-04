using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS.Entities;

namespace MS.DbContexts.Mappings
{
    public class TestTableMap : IEntityTypeConfiguration<TestTable>
    {
        public void Configure(EntityTypeBuilder<TestTable> builder)
        {
            builder.ToTable("TblTestTable");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.HasIndex(c => c.TestName).IsUnique();
            builder.Property(c => c.TestName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.IsAdmin).IsRequired().HasDefaultValue(true);
            builder.Property(c => c.CreateTime).IsRequired().HasDefaultValue(DateTime.Now);

        }
    }
}
