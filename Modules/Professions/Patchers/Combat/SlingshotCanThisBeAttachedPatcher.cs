namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
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

    /// <summary>Patch to allow Piper equipping Slime ammo.</summary>
    [HarmonyPostfix]
    private static void SlingshotCanThisBeAttachedPostfix(Slingshot __instance, ref bool __result, SObject? o)
    {
        if (__result || o is null || o.bigCraftable.Value)
        {
            return;
        }

        var lastUser = __instance.getLastFarmerToUse();
        __result = (o.ParentSheetIndex == ObjectIds.Slime && lastUser.HasProfession(VanillaProfession.Rascal)) ||
                   (o.ParentSheetIndex == ObjectIds.MonsterMusk && lastUser.HasProfession(VanillaProfession.Rascal, true));
    }

    #endregion harmony patches
}
