using IISManager.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISManager.Core.Infrastructure.Infrastructure.Persistance
{
	public class ApplicationContext : DbContext
	{
		public DbSet<AppPoolGroup> AppPoolGroups { get; set; }

		public ApplicationContext(DbContextOptions options) : base(options)
		{
		}
	}
}
