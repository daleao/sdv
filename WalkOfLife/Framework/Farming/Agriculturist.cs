using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeProfessions.Framework.Farming
{
	internal class Agriculturist
	{
		public List<int> FertilizerIds { get; } = new()
		{
			368,    // basic fertilizer
			369,    // quality fertilizer
			370,    // basic retaining soil
			371,    // quality retaining soil
			465,    // speed-gro
			466,    // deluxe speed-gro
			918,    // hyper speed-gro
			919,    // deluxe fertilizer
			920 // deluxe retaining soil
		};
	}
}
