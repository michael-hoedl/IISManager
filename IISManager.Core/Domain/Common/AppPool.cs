using IISManager.Core.Domain.Enums;
using Microsoft.Web.Administration;

namespace IISManager.Core.Domain.Common
{
	public class AppPool
	{
		public string Name { get; set; }
		public AppPoolStatus Status { get; set; }
		public List<AppPoolApplication> Applications { get; set; } = new List<AppPoolApplication>();

		public static AppPool Create(ApplicationPool pool)
		{
			try
			{

				return new AppPool
				{
					Name = pool.Name,
					Status = (AppPoolStatus)pool.State
				};
			}
			catch
			{
				return new AppPool();
			}
		}
	}
}
