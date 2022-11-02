namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterSetInvincibilityCountdownPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterSetInvincibilityCountdownPatch"/> class.</summary>
    internal MonsterSetInvincibilityCountdownPatch()
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.setInvincibleCountdown));
    }

    #region harmony patches

    /// <summary>Defense increases parry damage.</summary>
    [HarmonyPrefix]
    private static void MonsterSetInvincibilityCountdownPrefix(Monster __instance, ref int time)
    {
        if (ModEntry.Config.Arsenal.Weapons.ComboHits)
        {
            time /= 2;
        }
    }

    #endregion harmony patches
}
