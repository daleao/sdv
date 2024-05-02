namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    internal CrabPotCheckForActionPatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static void CrabPotCheckForActionPrefix(CrabPot __instance, bool justCheckingForActivity, ref SObject? __state)
    {
        if (!justCheckingForActivity && TweexModule.Config.TrashDoesNotConsumeBait &&
            __instance.heldObject.Value?.IsTrash() == true)
        {
            __state = __instance.bait.Value;
        }
    }

    [HarmonyPostfix]
    private static void CrabPotCheckForActionPostfix(CrabPot __instance, bool justCheckingForActivity, SObject? __state)
    {
        if (!justCheckingForActivity && __state is not null && __instance.bait.Value is null)
        {
            __instance.bait.Value = __state;
        }
    }

    #endregion harmony patches
}
