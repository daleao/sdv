namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using DaLion.Stardew.Arsenal.Framework.Events;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

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
    private static void FarmerCanMoveNowPostfix()
    {
        ModEntry.State.WeaponSwingCooldown = 16;
        ModEntry.Events.Enable<ComboResetUpdateTickedEvent>();
    }

    #endregion harmony patches
}
