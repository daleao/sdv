using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectPerformObjectDropInActionPatch : BasePatch
	{
		private static IReflectionHelper _reflection;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="reflection">Interface for accessing inaccessible code.</param>
		internal ObjectPerformObjectDropInActionPatch(ModConfig config, IMonitor monitor, IReflectionHelper reflection)
		: base(config, monitor)
		{
			_reflection = reflection;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
				prefix: new HarmonyMethod(GetType(), nameof(ObjectPerformObjectDropInActionPrefix))
			);
		}

		/// <summary>Patch to cut kiln wood consumption for Arborist.</summary>
		protected static bool ObjectPerformObjectDropInActionPrefix(ref SObject __instance, Item dropInItem, bool probe, Farmer who)
		{
			if (!__instance.name.Equals("Charcoal Kiln") || !Utils.PlayerHasProfession("arborist", who))
			{
				return true; // run original logic
			}

			SObject dropIn = dropInItem as SObject;
			if (who.IsLocalPlayer && (dropIn.ParentSheetIndex != 388 || dropIn.Stack < 5))
			{
				if (!probe && who.IsLocalPlayer && SObject.autoLoadChest == null)
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12783"));
				}
				return false;
			}
			if (__instance.heldObject.Value == null && !probe && dropIn.ParentSheetIndex == 388 && dropIn.Stack >= 5)
			{
				__instance.ConsumeInventoryItem(who, dropIn, 5);
				who.currentLocation.playSound("openBox");
				DelayedAction.playSoundAfterDelay("fireball", 50);
				__instance.showNextIndex.Value = true;
				_reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(27, __instance.TileLocation * 64f + new Vector2(-16f, -128f), Color.White, 4, flipped: false, 50f, 10, 64, (__instance.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f)
				{
					alphaFade = 0.005f
				});
				__instance.heldObject.Value = new SObject(382, 1);
				__instance.MinutesUntilReady = 30;
			}

			return false; // don't run original logic
		}
	}
}
