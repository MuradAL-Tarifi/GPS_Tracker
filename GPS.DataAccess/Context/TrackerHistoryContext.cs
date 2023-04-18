using GPS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Context
{
    public class TrackerHistoryContext : DbContext
    {
        protected TrackerHistoryContext() { }

        public TrackerHistoryContext(DbContextOptions<TrackerHistoryContext> options) : base(options) { }

        public virtual DbSet<InventoryHistory> InventoryHistory { get; set; }
    }
}
