namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsMonsterDamageApplicablePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationIsMonsterDamageApplicablePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationIsMonsterDamageApplicablePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>("isMonsterDamageApplicable");
    }

    #region harmony patches

    /// <summary>Patch to make Slimes immune to Piper damage.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GameLocationIsMonsterDamageApplicablePrefix(
        ref bool __result, Farmer who, Monster monster)
    {
        if (monster is not GreenSlime slime || slime.Get_Piped() is not { } piped ||
            piped.Piper != who)
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
