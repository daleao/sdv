namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanThisBeAttachedPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanThisBeAttachedPatch"/> class.</summary>
    internal SlingshotCanThisBeAttachedPatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.canThisBeAttached));
    }

    #region harmony patches

    /// <summary>Patch to allow Piper equipping Slime ammo.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        __result = __result || (o is { bigCraftable.Value: false, ParentSheetIndex: 766 } &&
                                __instance.getLastFarmerToUse().HasProfession(Profession.Piper));
    }

    #endregion harmony patches
}
