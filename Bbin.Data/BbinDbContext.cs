using Bbin.Api.Entitys;
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

            modelBuilder.Entity<ResultEntity>().ToTable("Result");

            modelBuilder.Entity<ResultEntity>(eb =>
            {
                eb.Property(b => b.End).HasColumnType("datetime");
                eb.ToTable("Result");
            });
        }
    }
}
