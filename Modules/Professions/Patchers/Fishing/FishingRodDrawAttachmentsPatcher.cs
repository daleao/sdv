namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodDrawAttachmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDrawAttachmentsPatcher"/> class.</summary>
    internal FishingRodDrawAttachmentsPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.drawAttachments));
    }

    #region harmony patches

    /// <summary>Draw tackle memory.</summary>
    [HarmonyPostfix]
    private static void FishingRodDrawAttachmentsPostfix(
        Tool __instance,
        SpriteBatch b,
        ref int x,
        ref int y)
    {
        if (__instance is not FishingRod { UpgradeLevel: > 2 } rod)
        {
            return;
        }

        var lastUsed = rod.Read<int>(DataKeys.LastTackleUsed);
        if (lastUsed == 0)
        {
            return;
        }

        var memorized = new SObject(lastUsed, 1);
        var transparency = rod.Read<float>(DataKeys.LastTackleUses) / FishingRod.maxTackleUses;
        memorized.drawInMenu(b, new Vector2(x + 68, y + 64 + 4), 1f, transparency, 0.9f);
    }

    #endregion harmony patches
}
