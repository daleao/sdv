namespace DaLion.Redux.Framework.Professions.Patches.Foraging;

#region using directives

using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCtorPatch"/> class.</summary>
    internal ObjectCtorPatch()
    {
        this.Target = this.RequireConstructor<SObject>(
            typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for Ecologist wild berry recovery.</summary>
    [HarmonyPostfix]
    private static void ObjectCtorPostfix(SObject __instance)
    {
        var owner = Game1.getFarmer(__instance.owner.Value);
        if (__instance.IsWildBerry() && owner.HasProfession(Profession.Ecologist))
        {
            __instance.Edibility =
                (int)(__instance.Edibility * (owner.HasProfession(Profession.Ecologist, true) ? 2f : 1.5f));
        }
    }

    #endregion harmony patches
}
