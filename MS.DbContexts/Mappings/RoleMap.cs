using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MS.Entities;
using MS.Entities.Core;

namespace MS.DbContexts.Mappings
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("TblRoles");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();//不自动生成值
            builder.HasIndex(c => c.Name).IsUnique();//指定索引，不能重复
            builder.Property(c => c.Name).IsRequired().HasMaxLength(16);
            builder.Property(c => c.DisplayName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Remark).HasMaxLength(4000);
            builder.Property(c => c.Creator).IsRequired();
            builder.Property(c => c.CreateTime).IsRequired();
            builder.Property(c => c.Modifier);
            builder.Property(c => c.ModifyTime);

            //builder.HasQueryFilter(b => b.StatusCode != StatusCode.delete);//默认不查询软删除数据
        }
    }
}
