namespace TheLion.Stardew.Professions.Framework.Events;

/// <summary>Base implementation for dynamic events.</summary>
internal abstract class BaseEvent : IEvent
{
    /// <inheritdoc />
    public abstract void Hook();

    /// <inheritdoc />
    public abstract void Unhook();
}