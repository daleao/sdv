namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class TemporaryAnimatedSpriteCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TemporaryAnimatedSpriteCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal TemporaryAnimatedSpriteCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
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

    /// <summary>Patch to allow manual detonation.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void TemporaryAnimatedSpriteCtorPostfix(TemporaryAnimatedSprite __instance, Farmer owner)
    {
        if (!owner.HasProfession(Profession.Demolitionist) || !State.IsManualDetonationModeEnabled)
        {
            return;
        }

        __instance.totalNumberOfLoops = int.MaxValue;
        EventManager.Enable<ManualDetonationUpdateTickedEvent>();
    }

    #endregion harmony patches
}
