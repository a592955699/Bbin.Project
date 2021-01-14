using Bbin.Core.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Data
{
    public class BbinDbContext : DbContext
    {
        public BbinDbContext(DbContextOptions<BbinDbContext> options) : base(options)
        {
        }

        public DbSet<RecommendTemplateEntity> RecommendTemplates { get; set; }
        public DbSet<RecommendItemEntity> RecommendItems { get; set; }

        public DbSet<GameEntity> Games { get; set; }
        public DbSet<ResultEntity> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置父类中的索引
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<GameEntity>(eb =>
            {
                eb.Property(b => b.DateTime).HasColumnType("datetime");
                eb.ToTable("Game");
                eb.HasIndex(c => new { c.RoomId, c.Index, c.Date });
            });

            modelBuilder.Entity<ResultEntity>(eb=> {
                eb.ToTable("Result");
                eb.HasIndex(c => c.Index);
            });

            modelBuilder.Entity<RecommendTemplateEntity>(eb=> {
                eb.ToTable("RecommendTemplate");                
            });

            modelBuilder.Entity<RecommendItemEntity>(eb=> {
                eb.ToTable("RecommendItem");
            });

            modelBuilder.Entity<ResultEntity>(eb =>
            {
                eb.Property(b => b.End).HasColumnType("datetime");
                eb.ToTable("Result");
            });
        }
    }
}
