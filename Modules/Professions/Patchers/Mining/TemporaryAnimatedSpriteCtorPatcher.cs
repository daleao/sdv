namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
        if (!owner.HasProfession(VanillaProfession.Demolitionist))
        {
            return;
        }

        __instance.bombRadius++;
        if (owner.HasProfession(VanillaProfession.Demolitionist, true))
        {
            __instance.bombRadius++;
        }

        if (!ProfessionsModule.Config.ModKey.IsDown())
        {
            return;
        }

        __instance.totalNumberOfLoops = int.MaxValue;
        EventManager.Enable<ManualDetonationUpdateTickedEvent>();
    }

    #endregion harmony patches
}
