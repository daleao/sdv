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
internal sealed class FarmerActiveObjectSetterPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerActiveObjectSetterPatch"/> class.</summary>
    internal FarmerActiveObjectSetterPatch()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.ActiveObject));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void FarmerActiveObjectSetterPostfix()
    {
    }

    #endregion harmony patches
}
