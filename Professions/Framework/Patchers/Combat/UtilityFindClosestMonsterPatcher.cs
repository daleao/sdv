namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityFindClosestMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityFindClosestMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal UtilityFindClosestMonsterPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.findClosestMonsterWithinRange));
    }

    #region harmony patches

    /// <summary>Modify filtering function.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FindClosestMonsterPrefix(ref Func<Monster, bool> match)
    {
        Func<Monster, bool> filter = (monster) => monster is GreenSlime slime && slime.Get_Piped() == null;
        if (match == null)
        {
            match = filter;
        }
        else
        {
            var prev = match;
            match = (monster) => prev(monster) && filter(monster);
        }

        return true;
    }

    #endregion harmony patches
}
