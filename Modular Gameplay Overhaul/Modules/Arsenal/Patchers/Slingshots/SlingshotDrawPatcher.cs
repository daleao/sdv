namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

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
        return !ArsenalModule.Config.Slingshots.BullseyeReplacesCursor;
    }

    #endregion harmony patches
}
