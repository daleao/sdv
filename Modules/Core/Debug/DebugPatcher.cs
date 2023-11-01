namespace DaLion.Overhaul.Modules.Core.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
        this.Target = this.RequireConstructor<MeleeWeapon>(typeof(int));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void DebugPrefix()
    {
        Log.D("Debug prefix called!");
    }

    [HarmonyPostfix]
    private static void DebugPostfix(MeleeWeapon __instance)
    {
        Log.A($"{__instance.Name}: {__instance.type.Value}");
        Log.D("Debug postfix called!");
    }

    #endregion harmony patches
}
