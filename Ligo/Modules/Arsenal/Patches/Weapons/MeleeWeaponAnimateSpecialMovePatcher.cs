namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponAnimateSpecialMovePatcher"/> class.</summary>
    internal MeleeWeaponAnimateSpecialMovePatcher()
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
