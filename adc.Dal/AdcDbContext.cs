using System;
using Microsoft.EntityFrameworkCore;
using adc.Entities;

namespace adc.Dal {
    public class AdcDbContext : DbContext {
        public AdcDbContext(DbContextOptions<AdcDbContext> options) : base(options) { }
        public virtual DbSet<Conversion> Conversions { get; set; }
    }
}
