using IISManager.Core.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISManager.Core.Domain.Common
{
	public class AppPoolGroup
	{
		[Key]
		public string Name { get; set; }

		public ICollection<AppPoolGroupAppPool> AssignedAppPoolNames { get; set; } = new List<AppPoolGroupAppPool>();
		
		public IEnumerable<AppPool> GetAppPools(IISManagerService manager)
		{
			var names = AssignedAppPoolNames.Select(x => x.Name.ToLower());
			return manager.GetAppPools().Where(x => names.Contains(x.Name.ToLower()));
		}
	}
}
