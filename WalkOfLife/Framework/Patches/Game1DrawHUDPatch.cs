using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class Game1DrawHUDPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal Game1DrawHUDPatch(IMonitor monitor)
		: base(monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), name: "drawHUD"),
				transpiler: new HarmonyMethod(GetType(), nameof(Game1DrawHUDTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(Game1DrawHUDPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
		protected static IEnumerable<CodeInstruction> Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Game1)}::drawHUD.");

			/// From: if (!player.professions.Contains(<scavenger_id>) || !currentLocation.IsOutdoors) return
			/// To: if (!(player.professions.Contains(<scavenger_id>) || player.professions.Contains(<prospector_id>)) return

			Label isProspector = iLGenerator.DefineLabel();
			try
			{
				_helper
					.FindProfessionCheck(Farmer.tracker)								// find index of tracker check
					.Retreat()
					.ToBufferUntil(
						new CodeInstruction(OpCodes.Brfalse)							// copy profession check
					)
					.InsertBuffer()														// paste
					.Return()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_S)
					)
					.SetOperand(Utility.ProfessionMap.Forward["prospector"])			// change to prospector check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)
					)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Brtrue_S, operand: isProspector)	// change !(A && B) to !(A || B)
					)
					.Advance()
					.StripLabels()														// strip repeated label
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Game1), nameof(Game1.currentLocation)).GetGetMethod())
					)
					.Remove(3)															// remove currentLocation.IsOutdoors check
					.AddLabel(isProspector);											// branch here is first profession check was true
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: if ((bool)pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590) ...
			/// To: if (_ShouldDraw(pair.Value)) ...

			try
			{
				_helper
					.FindNext(
						new CodeInstruction(OpCodes.Bne_Un)	// find branch to loop head
					)
					.GetOperand(out object loopHead)		// copy destination
					.RetreatUntil(
						#pragma warning disable AvoidNetField // Avoid Netcode types when possible
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SObject), nameof(SObject.isSpawnedObject)))
						#pragma warning restore AvoidNetField // Avoid Netcode types when possible
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Bne_Un)	// remove pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590
					)
					.Insert(								// insert call to custom condition
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Game1DrawHUDPatch), nameof(_ShouldDraw))),
						new CodeInstruction(OpCodes.Brfalse, operand: loopHead)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch for Prospector to track initial ladder down + draw ticks over trackable objects in view.</summary>
		protected static void Game1DrawHUDPostfix()
		{
			// track initial ladder down
			if (AwesomeProfessions.InitialLadderTiles.Count() > 0)
			{
				foreach (var tile in AwesomeProfessions.InitialLadderTiles)
				{
					if (StardewValley.Utility.isOnScreen(tile * 64f + new Vector2(32f, 32f), 64)) continue;

					Rectangle vpbounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
					Vector2 onScreenPosition = default;
					float rotation = 0f;
					if (tile.X * 64f > Game1.viewport.MaxCorner.X - 64)
					{
						onScreenPosition.X = vpbounds.Right - 8;
						rotation = (float)Math.PI / 2f;
					}
					else if (tile.X * 64f < Game1.viewport.X)
					{
						onScreenPosition.X = 8f;
						rotation = -(float)Math.PI / 2f;
					}
					else
						onScreenPosition.X = tile.X * 64f - Game1.viewport.X;

					if (tile.Y * 64f > Game1.viewport.MaxCorner.Y - 64)
					{
						onScreenPosition.Y = vpbounds.Bottom - 8;
						rotation = (float)Math.PI;
					}
					else if (tile.Y * 64f < Game1.viewport.Y)
						onScreenPosition.Y = 8f;
					else
						onScreenPosition.Y = tile.Y * 64f - Game1.viewport.Y;

					if (onScreenPosition.X == 8f && onScreenPosition.Y == 8f)
						rotation += (float)Math.PI / 4f;

					if (onScreenPosition.X == 8f && onScreenPosition.Y == vpbounds.Bottom - 8)
						rotation += (float)Math.PI / 4f;

					if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == 8f)
						rotation -= (float)Math.PI / 4f;

					if (onScreenPosition.X == vpbounds.Right - 8 && onScreenPosition.Y == vpbounds.Bottom - 8)
						rotation -= (float)Math.PI / 4f;

					Rectangle srcRect = new Rectangle(412, 495, 5, 4);
					float renderScale = 4f;
					Vector2 safePos = StardewValley.Utility.makeSafe(renderSize: new Vector2(srcRect.Width * renderScale, srcRect.Height * renderScale), renderPos: onScreenPosition);
					Game1.spriteBatch.Draw(Game1.mouseCursors, safePos, srcRect, Color.Cyan, rotation, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
				}
			}

			if (!AwesomeProfessions.Config.Modkey.IsDown()) return;

			// draw ticks over trackable objects in view
			Vector2 offset = new Vector2(0f, -33f);
			foreach (var kvp in Game1.currentLocation.Objects.Pairs)
			{
				if (!_ShouldDraw(kvp.Value) || !StardewValley.Utility.isOnScreen(kvp.Key * 64f + new Vector2(32f, 32f), 64)) continue;

				Rectangle srcRect = new Rectangle(412, 495, 5, 4);
				float renderScale = 5f;
				Vector2 targetPixel = new Vector2((kvp.Key.X * 64f) + 32f, (kvp.Key.Y * 64f) + 32f) + offset;
				Vector2 adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
				adjustedPixel = StardewValley.Utility.ModifyCoordinatesForUIScale(adjustedPixel);
				Game1.spriteBatch.Draw(Game1.mouseCursors, adjustedPixel, srcRect, Color.White, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
			}
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether the game should draw an arrow over a given object.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _ShouldDraw(SObject obj)
		{
			return (Utility.LocalPlayerHasProfession("scavenger") && ((obj.IsSpawnedObject && !Utility.IsForagedMineral(obj)) || obj.ParentSheetIndex == 590))
				|| (Utility.LocalPlayerHasProfession("prospector") && (Utility.IsResourceNode(obj) || Utility.IsForagedMineral(obj)));
		}
		#endregion private methods
	}
}
