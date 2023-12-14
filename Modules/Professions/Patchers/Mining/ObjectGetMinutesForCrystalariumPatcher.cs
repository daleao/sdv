namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetMinutesForCrystalariumPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectGetMinutesForCrystalariumPatcher"/> class.</summary>
    internal ObjectGetMinutesForCrystalariumPatcher()
    {
        this.Target = this.RequireMethod<SObject>("getMinutesForCrystalarium");
    }

    #region harmony patches

    /// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
    [HarmonyPostfix]
    private static void ObjectGetMinutesForCrystalariumPostfix(SObject __instance, ref int __result)
    {
        var owner = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : __instance.GetOwner();
        if (owner.HasProfession(VanillaProfession.Gemologist))
        {
            __result = (int)(__result * (owner.HasProfession(VanillaProfession.Gemologist, true) ? 0.5 : 0.75));
        }
    }

    #endregion harmony patches
}
