namespace DaLion.Ligo.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class TemporaryAnimatedSpriteCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TemporaryAnimatedSpriteCtorPatcher"/> class.</summary>
    internal TemporaryAnimatedSpriteCtorPatcher()
    {
        this.Target = this.RequireConstructor<TemporaryAnimatedSprite>(
            typeof(int),
            typeof(float),
            typeof(int),
            typeof(int),
            typeof(Vector2),
            typeof(bool),
            typeof(bool),
            typeof(GameLocation),
            typeof(Farmer));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist bomb radius + allow manual detonation.</summary>
    [HarmonyPostfix]
    private static void TemporaryAnimatedSpriteCtorPostfix(TemporaryAnimatedSprite __instance, Farmer owner)
    {
        if (!owner.HasProfession(Profession.Demolitionist))
        {
            return;
        }

        __instance.bombRadius++;
        if (owner.HasProfession(Profession.Demolitionist, true))
        {
            __instance.bombRadius++;
        }

        if (!ModEntry.Config.Professions.ModKey.IsDown())
        {
            return;
        }

        __instance.totalNumberOfLoops = int.MaxValue;
        ModEntry.Events.Enable<ManualDetonationUpdateTickedEvent>();
    }

    #endregion harmony patches
}
