namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

using DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatcher : HarmonyPatcher
{
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
        __result = __result || o?.ParentSheetIndex is ObjectIds.RadioactiveOre or ObjectIds.Emerald or ObjectIds.Aquamarine
                       or ObjectIds.Ruby or ObjectIds.Amethyst or ObjectIds.Topaz or ObjectIds.Jade or ObjectIds.Diamond or SObject.prismaticShardIndex ||
                   (JsonAssetsIntegration.GarnetIndex.HasValue && o?.ParentSheetIndex == JsonAssetsIntegration.GarnetIndex.Value);
    }

    #endregion harmony patches
}
