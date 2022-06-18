namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectCtorPatch()
    {
        Original = RequireConstructor<SObject>(typeof(Vector2), typeof(int), typeof(string), typeof(bool),
            typeof(bool), typeof(bool), typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for Ecologist wild berry recovery.</summary>
    [HarmonyPostfix]
    private static void ObjectCtorPostfix(SObject __instance)
    {
        var owner = Game1.getFarmer(__instance.owner.Value);
        if (__instance.IsWildBerry() && owner.HasProfession(Profession.Ecologist))
            __instance.Edibility =
                (int) (__instance.Edibility * (owner.HasProfession(Profession.Ecologist, true) ? 2f : 1.5f));
    }

    #endregion harmony patches
}