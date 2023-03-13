using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IISManager.Core.Domain.Common
{
	public class AppPoolGroupAppPool
	{
		[Key]
		public int ID { get; set; }

        public string Name { get; set; }
    }
}
