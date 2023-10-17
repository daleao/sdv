namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatcher : HarmonyPatcher
{
    private static readonly HashSet<int> ExtraAllowedAmmo = new()
    {
        ObjectIds.RadioactiveOre,
        ObjectIds.Emerald,
        ObjectIds.Aquamarine,
        ObjectIds.Ruby,
        ObjectIds.Amethyst,
        ObjectIds.Topaz,
        ObjectIds.Jade,
        ObjectIds.Diamond,
        ObjectIds.PrismaticShard,
    };

    /// <summary>Initializes a new instance of the <see cref="SlingshotCanThisBeAttachedPatcher"/> class.</summary>
    internal SlingshotCanThisBeAttachedPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.canThisBeAttached));
    }

    #region harmony patches

    /// <summary>Patch to allow equipping radioactive ore.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(ref bool __result, SObject? o)
    {
        if (!CombatModule.Config.EnableWeaponOverhaul || o is null || o.bigCraftable.Value)
        {
            return;
        }

        __result = __result || ExtraAllowedAmmo.Contains(o.ParentSheetIndex) ||
                   (JsonAssetsIntegration.GarnetIndex.HasValue &&
                    o.ParentSheetIndex == JsonAssetsIntegration.GarnetIndex.Value);
    }

    #endregion harmony patches
}
