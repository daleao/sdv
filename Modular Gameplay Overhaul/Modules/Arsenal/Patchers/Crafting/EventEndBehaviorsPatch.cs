namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using DaLion.Shared.Harmony;
using Events;
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
        if (ArsenalModule.Config.DwarvishCrafting && __instance.id == Constants.ForgeIntroQuestId)
        {
            EventManager.Enable<BlueprintDayStartedEvent>();
        }
    }

    #endregion harmony patches
}
