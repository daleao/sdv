namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

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
        __result = __result || o?.ParentSheetIndex is ItemIDs.RadioactiveOre or ItemIDs.Emerald or ItemIDs.Aquamarine
                       or ItemIDs.Ruby or ItemIDs.Amethyst or ItemIDs.Topaz or ItemIDs.Jade or ItemIDs.Diamond or SObject.prismaticShardIndex ||
                   (Globals.GarnetIndex.HasValue && o?.ParentSheetIndex == Globals.GarnetIndex.Value);
    }

    #endregion harmony patches
}
