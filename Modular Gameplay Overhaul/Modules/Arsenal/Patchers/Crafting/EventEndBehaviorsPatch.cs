namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class EventEndBehaviorsPatch : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventEndBehaviorsPatch"/> class.</summary>
    internal EventEndBehaviorsPatch()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.endBehaviors));
    }

    #region harmony patches

    /// <summary>Subscribe to blueprint translation event.</summary>
    [HarmonyPostfix]
    private static void EventEndBehaviorsPostfix(Event __instance)
    {
        if (__instance.id == (int)Quest.ForgeIntro)
        {
            EventManager.Enable<BlueprintDayStartedEvent>();
        }
    }

    #endregion harmony patches
}
