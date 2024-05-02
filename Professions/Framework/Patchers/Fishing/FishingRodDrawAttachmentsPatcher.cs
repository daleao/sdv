namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodDrawAttachmentsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
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
        if (__instance is not FishingRod { UpgradeLevel: > 2, numAttachmentSlots.Value: > 1 } rod)
        {
            return;
        }

        var memorizedTackle = Data.Read(rod, DataKeys.FirstMemorizedTackle);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            var memorized = ItemRegistry.Create<SObject>(memorizedTackle);
            var transparency = 2f * Data.ReadAs<float>(rod, DataKeys.FirstMemorizedTackleUses) / FishingRod.maxTackleUses;
            memorized.drawInMenu(b, new Vector2(x + 68, y + 68), 1f, transparency, 0.9f);
        }

        if (rod.AttachmentSlotsCount < 3)
        {
            return;
        }

        memorizedTackle = Data.Read(rod, DataKeys.SecondMemorizedTackle);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            var memorized = ItemRegistry.Create<SObject>(memorizedTackle);
            var transparency = 2f * Data.ReadAs<float>(rod, DataKeys.SecondMemorizedTackleUses) / FishingRod.maxTackleUses;
            memorized.drawInMenu(b, new Vector2(x + 68, y + 136), 1f, transparency, 0.9f);
        }
    }

    #endregion harmony patches
}
