namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetMinutesForCrystalariumPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ObjectGetMinutesForCrystalariumPatch"/> class.</summary>
    internal ObjectGetMinutesForCrystalariumPatch()
    {
        this.Target = this.RequireMethod<SObject>("getMinutesForCrystalarium");
    }

    #region harmony patches

    /// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
    [HarmonyPostfix]
    private static void ObjectGetMinutesForCrystalariumPostfix(SObject __instance, ref int __result)
    {
        var owner = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : __instance.GetOwner();
        if (owner.HasProfession(Profession.Gemologist))
        {
            __result = (int)(__result * (owner.HasProfession(Profession.Gemologist, true) ? 0.5 : 0.75));
        }
    }

    #endregion harmony patches
}
