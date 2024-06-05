namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationDamageMonsterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            [
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Steadfast enchantment crit. to damage conversion.</summary>
    [HarmonyPrefix]
    private static void GameLocationDamageMonsterPrefix(bool __result, Farmer who)
    {
        if (who.IsLocalPlayer && __result)
        {
            State.SecondsOutOfCombat = 0;
        }
    }

    #endregion harmony patches
}
