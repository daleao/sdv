namespace DaLion.Ligo.Modules.Core.Patchers;

#region using directives

using DaLion.Ligo.Modules.Core.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    internal GameLocationDamageMonsterPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Reset seconds out of combat.</summary>
    [HarmonyPostfix]
    private static void GameLocationDamageMonsterPostfix(Farmer __instance)
    {
        __instance.Set_SecondsOutOfCombat(0);
    }

    #endregion harmony patches
}
