namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class StaticSavingEvent : SavingEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticSavingEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnSavingImpl(object sender, SavingEventArgs e)
    {
        ModData.CleanUpRogueData();
    }
}