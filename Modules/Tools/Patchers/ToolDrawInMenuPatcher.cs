namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
[ModConflict("spacechase0.MoonMisadventures")]
internal sealed class ToolDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawInMenuPatcher"/> class.</summary>
    internal ToolDrawInMenuPatcher()
    {
        this.Target = this.RequireMethod<Tool>(
            nameof(Tool.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Replace tool texture.</summary>
    [HarmonyPrefix]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Bypass property setter.")]
    private static void ToolDrawInMenuPrefix(Tool __instance, ref (int, Texture2D)? __state)
    {
        if (__instance.UpgradeLevel < 5)
        {
            return;
        }

        __state = (__instance.UpgradeLevel, Game1.toolSpriteSheet);
        __instance.upgradeLevel.Value = 4;
        Reflector.GetStaticFieldSetter<Texture2D>(typeof(Game1), "_toolSpriteSheet")
            .Invoke(Textures.RadioactiveToolsTx);
    }

    /// <summary>Restore tool texture.</summary>
    [HarmonyPostfix]
    [SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Bypass property setter.")]
    private static void ToolDrawInMenuPostfix(Tool __instance, (int, Texture2D)? __state)
    {
        if (!__state.HasValue)
        {
            return;
        }

        __instance.upgradeLevel.Value = __state.Value.Item1;
        Reflector.GetStaticFieldSetter<Texture2D>(typeof(Game1), "_toolSpriteSheet")
            .Invoke(__state.Value.Item2);
    }

    #endregion harmony patches
}
