using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class TestPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal TestPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(BasicProjectile), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool), typeof(BasicProjectile.onCollisionBehavior) } ),
				prefix: new HarmonyMethod(GetType(), nameof(TestPrefix)),
				postfix: new HarmonyMethod(GetType(), nameof(TestPostfix))
			);
		}

		protected static bool TestPrefix()
		{
			return true;
		}

		protected static void TestPostfix(ref BasicProjectile __instance, ref NetInt ___bouncesLeft)
		{
		}
	}
}
