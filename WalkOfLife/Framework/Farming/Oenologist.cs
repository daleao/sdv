using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeProfessions.Framework.Farming
{
	internal class Oenologist
	{
		public List<int> WineFruitIds { get; } = new()
		{
			88,		// coconut
			90,		// cactus fruit
			252,	// rhubarb
			254,	// melon
			258,	// blueberry
			260,	// hot pepper
			268,	// starfruit
			282,	// cranberries
			296,	// salmonberry
			396,	// spice berry
			398,	// grape
			400,	// strawberry
			406,	// wild plum
			410,	// blackberry
			414,	// crystal fruit
			454,	// ancient fruit
			613,	// apple
			634,	// apricot
			635,	// orange
			636,	// peach
			637,	// pomegranate
			638,	// cherry
			0,		// pineapple ?
			0,		// mango ?
			0		// banana ?
		}; // see for Tiller details https://stardewcommunitywiki.com/Fruits
	}
}
