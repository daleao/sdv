namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatch"/> class.</summary>
    internal FarmerTakeDamagePatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Grant i-frames during stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool FarmerTakeDamagePrefix(Farmer __instance)
    {
        return __instance.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.stabbingSword, isOnSpecial: true };
    }

    #endregion harmony patches
}
