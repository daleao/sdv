namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.DayStarted;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class EventEndBehaviorsPatch : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventEndBehaviorsPatch"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal EventEndBehaviorsPatch(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.endBehaviors));
    }

    #region harmony patches

    /// <summary>Subscribe to blueprint translation event.</summary>
    [HarmonyPostfix]
    private static void EventEndBehaviorsPostfix(Event __instance)
    {
        if (__instance.id == (int)QuestId.ForgeIntro)
        {
            EventManager.Enable<BlueprintDayStartedEvent>();
        }
    }

    #endregion harmony patches
}
