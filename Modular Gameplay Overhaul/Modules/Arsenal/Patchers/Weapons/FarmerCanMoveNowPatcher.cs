namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

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
        if (who.IsLocalPlayer)
        {
            who.Set_IsAnimating(false);
        }
    }

    #endregion harmony patches
}
