namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Events;
using HarmonyLib;
using Shared.Harmony;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanMoveNowPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanMoveNowPatcher"/> class.</summary>
    internal FarmerCanMoveNowPatcher()
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
