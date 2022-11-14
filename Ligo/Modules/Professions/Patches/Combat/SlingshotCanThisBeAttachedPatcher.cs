namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using HarmonyLib;
using Shared.Harmony;
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

    /// <summary>Patch to allow Piper equipping Slime ammo.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        __result = __result || (o is { bigCraftable.Value: false, ParentSheetIndex: Constants.SlimeIndex } &&
                                __instance.getLastFarmerToUse().HasProfession(Profession.Rascal));
    }

    #endregion harmony patches
}
