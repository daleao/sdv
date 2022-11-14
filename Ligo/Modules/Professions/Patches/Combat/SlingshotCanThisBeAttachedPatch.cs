namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
        __result = __result || (o is { bigCraftable.Value: false, ParentSheetIndex: Constants.SlimeIndex } &&
                                __instance.getLastFarmerToUse().HasProfession(Profession.Rascal));
    }

    #endregion harmony patches
}
