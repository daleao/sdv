namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[ImplicitIgnore]
internal sealed class MonsterSetInvincibilityCountdownPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterSetInvincibilityCountdownPatcher"/> class.</summary>
    internal MonsterSetInvincibilityCountdownPatcher()
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.setInvincibleCountdown));
    }

    #region harmony patches

    /// <summary>Reduce monster invincibility.</summary>
    [HarmonyPrefix]
    private static void MonsterSetInvincibilityCountdownPrefix(Monster __instance, ref int time)
    {
        if (ModEntry.Config.Arsenal.Weapons.AllowComboHits)
        {
            time /= 2;
        }
    }

    #endregion harmony patches
}
