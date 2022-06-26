namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class TemporaryAnimatedSpriteCtorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TemporaryAnimatedSpriteCtorPatch()
    {
        Target = RequireConstructor<TemporaryAnimatedSprite>(typeof(int), typeof(float), typeof(int), typeof(int),
            typeof(Vector2), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Farmer));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist bomb radius.</summary>
    [HarmonyPostfix]
    private static void TemporaryAnimatedSpriteCtorPostfix(TemporaryAnimatedSprite __instance, Farmer owner)
    {
        if (owner.HasProfession(Profession.Demolitionist)) ++__instance.bombRadius;
        if (owner.HasProfession(Profession.Demolitionist, true)) ++__instance.bombRadius;
    }

    #endregion harmony patches
}