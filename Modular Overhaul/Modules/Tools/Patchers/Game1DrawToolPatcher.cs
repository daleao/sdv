namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
[IgnoreWithMod("spacechase0.MoonMisadventures")]
internal sealed class Game1DrawToolPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawToolPatcher"/> class.</summary>
    internal Game1DrawToolPatcher()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.drawTool), new[] { typeof(Farmer), typeof(int) });
    }

    #region harmony patches

    /// <summary>Replace tool texture.</summary>
    [HarmonyPrefix]
    private static void ToolDrawInMenuPrefix(Farmer f, ref (int, Texture2D)? __state)
    {
        var tool = f.CurrentTool;
        if (tool is null || tool.UpgradeLevel < 5)
        {
            return;
        }

        __state = (tool.UpgradeLevel, Game1.toolSpriteSheet);
        tool.upgradeLevel.Value = 4;
        Reflector.GetStaticFieldSetter<Texture2D>(typeof(Game1), "_toolSpriteSheet")
            .Invoke(Textures.RadioactiveToolsTx);
    }

    /// <summary>Restore tool texture.</summary>
    [HarmonyPostfix]
    private static void ToolDrawInMenuPostfix(Farmer f, (int, Texture2D)? __state)
    {
        if (!__state.HasValue)
        {
            return;
        }

        f.CurrentTool.upgradeLevel.Value = __state.Value.Item1;
        Reflector.GetStaticFieldSetter<Texture2D>(typeof(Game1), "_toolSpriteSheet")
            .Invoke(__state.Value.Item2);
    }

    #endregion harmony patches
}
