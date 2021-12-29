using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class ObjectCtorPatch : BasePatch
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
    private static void ObjectCtorPostfix(ref SObject __instance)
    {
        var owner = Game1.getFarmer(__instance.owner.Value);
        if (__instance.IsWildBerry() && owner.HasProfession("Ecologist"))
            __instance.Edibility =
                (int) (__instance.Edibility * (owner.HasPrestigedProfession("Ecologist") ? 2f : 1.5f));
    }

    #endregion harmony patches
}