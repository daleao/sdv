namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
//[ImplicitIgnore]
internal sealed class DebugPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatcher"/> class.</summary>
    internal DebugPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPrefix]
    private static bool DebugPrefix(object __instance)
    {
        //var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        //Log.D($"{caller} prefix called!");
        return true;
    }

    /// <summary>Placeholder patch for debugging.</summary>
    [HarmonyPostfix]
    private static void DebugPostfix(Slingshot __instance, SpriteBatch b)
    {
        //var caller = new StackTrace().GetFrame(1)?.GetMethod()?.GetFullName();
        //Log.D($"{caller} postfix called!");
    }

    #endregion harmony patches
}
