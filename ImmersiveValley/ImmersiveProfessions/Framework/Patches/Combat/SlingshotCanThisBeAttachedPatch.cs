namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotCanThisBeAttachedPatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.canThisBeAttached));
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        if (o is null || o.bigCraftable.Value) return;

        switch (o.ParentSheetIndex)
        {
            case 909: // radioactive ore
            case 766 when __instance.getLastFarmerToUse().HasProfession(Profession.Piper): // slime
                __result = true;
                break;
        }
    }

    #endregion harmony patches
}