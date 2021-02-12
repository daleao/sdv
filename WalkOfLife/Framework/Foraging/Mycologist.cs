using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeProfessions.Framework.Foraging
{
	internal class Mycologist
	{
		public List<int> MushroomIds { get; } = new()
		{
			257,	// morel
			281,	// chanterelle
			404,	// common mushroom
			420,	// red mushroom
			422,	// purple mushroom
			430,	// truffle
			0,		// magma cap ?
		};
	}
}
