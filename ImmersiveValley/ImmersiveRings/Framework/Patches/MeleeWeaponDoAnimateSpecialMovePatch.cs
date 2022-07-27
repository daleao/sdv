namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common.ModData;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatch()
    {
        Target = RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
        Postfix!.after = new[] { "DaLion.ImmersiveArsenal" };
    }

    #region harmony patches

    /// <summary>Implement Topaz enchantment CDR.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.ImmersiveArsenal")]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        if (ModEntry.Config.TopazPerk != ModConfig.Perk.Cooldown) return;

        var cdr = ModDataIO.Read<float>(__instance.getLastFarmerToUse(), "CooldownReduction");
        if (cdr <= 0f) return;

        if (MeleeWeapon.attackSwordCooldown > 0)
            MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown * (1f - cdr));

        if (MeleeWeapon.defenseCooldown > 0)
            MeleeWeapon.defenseCooldown = (int)(MeleeWeapon.defenseCooldown * (1f - cdr));

        if (MeleeWeapon.daggerCooldown > 0)
            MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * (1f - cdr));

        if (MeleeWeapon.clubCooldown > 0)
            MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * (1f - cdr));
    }

    #endregion harmony patches
}