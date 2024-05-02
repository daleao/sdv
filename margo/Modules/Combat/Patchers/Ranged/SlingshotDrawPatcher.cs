namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawPatcher"/> class.</summary>
    internal SlingshotDrawPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Override bullseye.</summary>
    [HarmonyPrefix]
    private static bool SlingshotDrawPrefix()
    {
        return !CombatModule.Config.ControlsUi.BullseyeReplacesCursor;
    }

    #endregion harmony patches
}
