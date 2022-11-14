namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Events;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponAnimateSpecialMovePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponAnimateSpecialMovePatch"/> class.</summary>
    internal MeleeWeaponAnimateSpecialMovePatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.animateSpecialMove));
    }

    #region harmony patches

    /// <summary>Trigger stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponAnimateSpecalMovePrefix(MeleeWeapon __instance, ref Farmer ___lastUser, Farmer who)
    {
        if (__instance.type.Value != MeleeWeapon.stabbingSword || MeleeWeapon.attackSwordCooldown > 0 ||
            !ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords)
        {
            return true; // run original logic
        }

        ___lastUser = who;
        ModEntry.Events.Enable<StabbySwordSpecialUpdateTickingEvent>();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
