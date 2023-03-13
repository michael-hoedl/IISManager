using IISManager.Core.Domain.Common;
using IISManager.Core.Domain.Enums;
using IISManager.Core.Infrastructure.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.Administration;
using System.Text.RegularExpressions;

namespace IISManager.Core.Infrastructure.Services
{
	public class IISManagerService
	{
		private readonly ServerManager manager;
		private readonly IDbContextFactory<ApplicationContext> contextFactory;

		public IISManagerService(IDbContextFactory<ApplicationContext> contextFactory)
		{
			this.manager = new ServerManager();
			this.contextFactory = contextFactory;
		}

		public IEnumerable<AppPool> GetAppPools()
		{
			var applications = manager.Sites.SelectMany(x => x.Applications).GroupBy(x => x.ApplicationPoolName).ToDictionary(x => x.Key.ToLower(), y => y.ToList());

			var retVal = new List<AppPool>();

			foreach (var a in manager.ApplicationPools)
			{
				var toAdd = new AppPool()
				{
					Name = a.Name,
					Status = (AppPoolStatus)a.State
				};

				if (applications.TryGetValue(toAdd.Name.ToLower(), out var apps))
				{
					toAdd.Applications = apps.Select(x => new AppPoolApplication()
					{
						Path = x.Path,
						PhysicalPath = x.VirtualDirectories["/"].PhysicalPath
					}).ToList();
				}

				retVal.Add(toAdd);
			}

			return retVal;
		}

		public async Task<IEnumerable<AppPoolGroup>> GetAppPoolGroups()
		{
			using var context = contextFactory.CreateDbContext();
			return await context.AppPoolGroups
				.Include(x => x.AssignedAppPoolNames)
				.ToListAsync();
		}

		public void SetState(AppPoolGroup group, AppPoolActionState state)
		{
			foreach (var pool in group.GetAppPools(this))
			{
				SetState(pool, state);
			}
		}

		public void SetState(AppPool pool, AppPoolActionState state)
		{
			var foundPool = manager.ApplicationPools.FirstOrDefault(x => x.Name == pool.Name);

			if (foundPool is not null)
			{
				switch (state)
				{
					case AppPoolActionState.Start when foundPool.State == ObjectState.Stopped:
						foundPool.Start();
						break;
					case AppPoolActionState.Stop when foundPool.State == ObjectState.Started:
						foundPool.Stop();
						break;
					case AppPoolActionState.Recycle when foundPool.State == ObjectState.Started:
						foundPool.Recycle();
						break;
					default:
						break;
				}
			}
		}

		public bool AppPoolExists(string name)
		{
			if(manager.ApplicationPools.FirstOrDefault(x => string.Compare(x.Name, name, true) == 0) is null)
			{
				return false;
			}

			return true;
		}

		public void CreateAppPool(string name, bool startImmediatley)
		{
			if (manager.ApplicationPools.FirstOrDefault(x => string.Compare(x.Name, name, true) == 0) is null)
			{
				var addedPool = manager.ApplicationPools.Add(name);
				manager.CommitChanges();

				if (!startImmediatley)
				{
					addedPool.Stop();
				}
			}
			
		}

		public async Task CreateLogicalGroup(string name)
		{
			using var context = await contextFactory.CreateDbContextAsync();
			await context.AppPoolGroups.AddAsync(new AppPoolGroup { Name = name });
			await context.SaveChangesAsync();
		}

		public void DeleteAppPool(AppPool pool)
		{
			var foundElement = manager.ApplicationPools.FirstOrDefault(x => x.Name == pool.Name);

			if(foundElement is not null) 
			{
				manager.ApplicationPools.Remove(foundElement);
			}
		}

		public async Task UpdateAppPoolGroup(AppPoolGroup group)
		{
			using var context = await contextFactory.CreateDbContextAsync();
			var x = context.AppPoolGroups.Update(group);
			await context.SaveChangesAsync();
		}

		public async Task DeleteAppPoolGroup(AppPoolGroup group)
		{
			using var context = await contextFactory.CreateDbContextAsync();
			context.AppPoolGroups.Remove(group);
			await context.SaveChangesAsync();
		}
	}
}
