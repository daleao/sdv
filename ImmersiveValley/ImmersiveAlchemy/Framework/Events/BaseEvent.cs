namespace DaLion.Stardew.Alchemy.Framework.Events;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation for an event wrapper allowing dynamic enabling / disabling.</summary>
internal abstract class BaseEvent : IEvent
{
    protected readonly PerScreen<bool> enabled = new();

    /// <inheritdoc />
    public bool IsEnabled => enabled.Value;

    /// <inheritdoc />
    public bool IsEnabledForScreen(int screenId)
    {
        return enabled.GetValueForScreen(screenId);
    }

    /// <inheritdoc />
    public void Enable()
    {
        enabled.Value = true;
    }

    /// <inheritdoc />
    public void Disable()
    {
        enabled.Value = false;
    }
}