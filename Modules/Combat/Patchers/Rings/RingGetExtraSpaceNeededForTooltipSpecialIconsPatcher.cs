namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using System.Collections.Generic;
using System.Net.NetworkInformation;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using static System.Net.Mime.MediaTypeNames;

#endregion using directives

[UsedImplicitly]
internal sealed class RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher : HarmonyPatcher
{
    private static HashSet<int>? _ids;

    /// <summary>Initializes a new instance of the <see cref="RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher"/> class.</summary>
    internal RingGetExtraSpaceNeededForTooltipSpecialIconsPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.getExtraSpaceNeededForTooltipSpecialIcons));
    }

    internal static int MinWidth { private get; set; }

    #region harmony patches

    /// <summary>Fix combined Infinity Band tooltip box height.</summary>
    [HarmonyPostfix]
    private static void RingGetExtraSpaceNeededForTooltipSpecialIconsPostfix(Ring __instance, ref Point __result, SpriteFont font)
    {
        if (__instance.IsCombinedInfinityBand(out var band))
        {
            __result.X = Math.Max(__result.X, MinWidth + 86);
            __result.Y += (int)(Math.Max(font.MeasureString("TT").Y, 48f) *
                                band.Get_StatBuffer().Count());
            if (band.Get_Chord()?.Root is not null)
            {
                __result.Y += 2 * (int)font.MeasureString("T").Y;
            }

            return;
        }

        if (!CombatModule.Config.RebalancedRings)
        {
            return;
        }

        if (_ids is null)
        {
            _ids = new HashSet<int>
            {
                ObjectIds.RubyRing,
                ObjectIds.AquamarineRing,
                ObjectIds.AmethystRing,
                ObjectIds.EmeraldRing,
                ObjectIds.JadeRing,
                ObjectIds.TopazRing,
                ObjectIds.SmallGlowRing,
                ObjectIds.GlowRing,
                ObjectIds.SmallMagnetRing,
                ObjectIds.MagnetRing,
                ObjectIds.GlowstoneRing,
                ObjectIds.CrabshellRing,
                //ObjectIds.ImmunityRing,
            };

            if (JsonAssetsIntegration.GarnetRingIndex.HasValue)
            {
                _ids.Add(JsonAssetsIntegration.GarnetRingIndex.Value);
            }
        }

        if (__instance is CombinedRing combined)
        {
            var originalDescriptionText = __instance.getDescription();
            __result.Y -= (int)font.MeasureString(originalDescriptionText).Y;
            for (var i = 0; i < combined.combinedRings.Count; i++)
            {
                var ring = combined.combinedRings[i];
                var descriptionText = ring.getDescription();
                var descriptionSize = font.MeasureString(descriptionText);
                var titleWidth = font.MeasureString(combined.DisplayName).X;
                descriptionSize.X = Math.Max(descriptionSize.X - 48f, titleWidth);

                if (ring.indexInTileSheet.Value == ObjectIds.GlowstoneRing)
                {
                    var parsedDescription = Game1.parseText(
                        Game1.objectInformation[ObjectIds.GlowRing].Split('/')[5],
                        Game1.smallFont,
                        (int)descriptionSize.X);
                    var fontHeight = font.MeasureString(parsedDescription).Y;
                    descriptionSize.Y = Math.Max(fontHeight, 48f);
                    __result.Y += (int)descriptionSize.Y;

                    parsedDescription = Game1.parseText(
                        Game1.objectInformation[ObjectIds.MagnetRing].Split('/')[5],
                        Game1.smallFont,
                        (int)descriptionSize.X);
                    fontHeight = font.MeasureString(parsedDescription).Y;
                    descriptionSize.Y = Math.Max(fontHeight, 48f);
                    __result.Y += (int)descriptionSize.Y;
                }
                else
                {
                    var parsedDescription = Game1.parseText(ring.description, Game1.smallFont, (int)descriptionSize.X);
                    var fontHeight = font.MeasureString(parsedDescription).Y;
                    descriptionSize.Y = Math.Max(fontHeight, 48f);
                    __result.Y += (int)descriptionSize.Y;
                }
            }

            return;
        }

        if (_ids.Contains(__instance.indexInTileSheet.Value))
        {
            var descriptionText = __instance.getDescription();
            __result.Y -= (int)font.MeasureString(descriptionText).Y;

            var descriptionSize = font.MeasureString(descriptionText);
            var titleWidth = font.MeasureString(__instance.DisplayName).X;
            descriptionSize.X = Math.Max(descriptionSize.X - 48f, titleWidth);

            var parsedDescription =
                Game1.parseText(__instance.description, Game1.smallFont, (int)descriptionSize.X);
            var fontHeight = font.MeasureString(parsedDescription).Y;
            descriptionSize.Y = Math.Max(fontHeight, 48f);
            __result.Y += (int)descriptionSize.Y;
        }
    }

    #endregion harmony patches
}
