namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Arsenal.Framework.Events;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolSetterPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolSetterPatch"/> class.</summary>
    internal FarmerCurrentToolSetterPatch()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentTool));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void FarmerCurrentToolSetterPostfix()
    {
        ModEntry.State.ComboHitStep = ComboHitStep.Idle;
    }

    #endregion harmony patches
}
