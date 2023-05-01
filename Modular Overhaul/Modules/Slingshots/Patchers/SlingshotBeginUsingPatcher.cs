namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotBeginUsingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotBeginUsingPatcher"/> class.</summary>
    internal SlingshotBeginUsingPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.beginUsing));
    }

    #region harmony patches

    /// <summary>Override bullseye.</summary>
    [HarmonyPostfix]
    private static void SlingshotBeginUsingPostfix()
    {
        if (SlingshotsModule.Config.BullseyeReplacesCursor)
        {
            EventManager.Enable<BullseyeRenderedEvent>();
        }
    }

    #endregion harmony patches
}
