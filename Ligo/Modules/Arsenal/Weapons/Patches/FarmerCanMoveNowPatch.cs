namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Events;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanMoveNowPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanMoveNowPatch"/> class.</summary>
    internal FarmerCanMoveNowPatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.canMoveNow));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void FarmerCanMoveNowPostfix(Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon || ModEntry.State.Arsenal.ComboHitStep == ComboHitStep.Idle)
        {
            return;
        }

        ModEntry.State.Arsenal.WeaponSwingCooldown = 400 - (weapon.speed.Value * 20);
        ModEntry.State.Arsenal.WeaponSwingCooldown *= (int)(1f - who.weaponSpeedModifier);
        ModEntry.State.Arsenal.WeaponSwingCooldown /= 15;
        ModEntry.Events.Enable<ComboResetUpdateTickedEvent>();
    }

    #endregion harmony patches
}
