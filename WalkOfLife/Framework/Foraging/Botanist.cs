using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeProfessions.Framework.Foraging
{
	internal class Botanist
	{
		public List<int> ForagePlantIds { get; } = new()
		{
			16,		// wild horseradish
			18,		// daffodil
			20,		// leek
			22,		// dandelion
			78,     // cave carrot
			88,     // coconut
			90,     // cactus fruit
			259,	// fiddlehead fern
			283,    // holly
			296,    // salmonberry
			396,    // spice berry
			398,    // grape
			388,	// spring onion
			402,    // sweet pea
			406,    // wild plum
			408,	// hazelnut
			410,    // blackberry
			414,    // crystal fruit
			412,	// winter root
			416,	// snow yam
			418,	// crocus
			0,		// ginger ?
		};
	}
}
