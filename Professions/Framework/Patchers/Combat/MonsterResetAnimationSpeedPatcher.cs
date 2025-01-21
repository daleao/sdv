namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterResetAnimationSpeedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterResetAnimationSpeedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MonsterResetAnimationSpeedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Monster>("resetAnimationSpeed");
    }

    #region harmony patches

    /// <summary>Patch to prevent damage convulsive Slimes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MonsterOverlapsFarmerForDamagePrefix(Monster __instance)
    {
        return __instance is not GreenSlime slime || slime.Get_Piped() is null;
    }

    #endregion harmony patches
}
