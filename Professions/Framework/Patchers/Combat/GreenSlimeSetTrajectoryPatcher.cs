namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeSetTrajectoryPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeSetTrajectoryPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeSetTrajectoryPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Character>(nameof(Character.setTrajectory), [typeof(int), typeof(int)]);
    }

    #region harmony patches

    /// <summary>Patch to increase jump velocity of inflated Slimes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void GreenSlimeSetTrajectoryPrefix(Character __instance, ref int xVelocity, ref int yVelocity)
    {
        if (__instance is not GreenSlime slime)
        {
            return;
        }

        xVelocity = (int)(xVelocity * slime.Scale);
        yVelocity = (int)(yVelocity * slime.Scale);
    }

    #endregion harmony patches
}
