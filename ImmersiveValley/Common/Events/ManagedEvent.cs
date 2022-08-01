namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;
using System.Runtime.CompilerServices;

#endregion using directives

/// <summary>Base implementation of an event wrapper allowing dynamic enabling / disabling.</summary>
internal abstract class ManagedEvent : IManagedEvent, IEquatable<ManagedEvent>
{
    /// <summary>Whether this event is enabled on each screen.</summary>
    private readonly PerScreen<bool> _Enabled = new();

    /// <summary>The <see cref="EventManager"/> instance that manages this event.</summary>
    protected EventManager Manager { get; init; }

    /// <inheritdoc cref="EventPriority"/>
    protected EventPriority Priority { get; init; }

    /// <summary>Allow this event to be raised even when disabled.</summary>
    protected bool AlwaysEnabled { get; init; } = false;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ManagedEvent(EventManager manager)
    {
        Manager = manager;
    }

    /// <summary>Invoked once when the event is enabled.</summary>
    protected virtual void OnEnabled() { }

    /// <summary>Invoked once when the event is disabled.</summary>
    protected virtual void OnDisabled() { }

    /// <inheritdoc />
    public virtual bool IsEnabled => _Enabled.Value || AlwaysEnabled;

    /// <inheritdoc />
    public bool IsEnabledForScreen(int screenID) => _Enabled.GetValueForScreen(screenID);

    /// <inheritdoc />
    public bool Enable()
    {
        if (_Enabled.Value || !(_Enabled.Value = true)) return false;

        OnEnabled();
        return true;
    }

    /// <inheritdoc />
    public bool Disable()
    {
        if (!_Enabled.Value || (_Enabled.Value = false)) return false;
        
        OnDisabled();
        return true;
    }

    /// <inheritdoc />
    public override string ToString() => GetType().Name;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => GetType().GetHashCode();

    /// <summary>Determines if the specified <see cref="ManagedEvent"/> is equal to the current instance.</summary>
    /// <param name="other">A <see cref="ManagedEvent"/> to compare to this instance.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> has the same type as this instance; otherwise, <see langword="false"/>.</returns>
    // ReSharper disable once CheckForReferenceEqualityInstead.1
    public virtual bool Equals(ManagedEvent? other) => GetType().Equals(other?.GetType());

    /// <inheritdoc />
    public override bool Equals(object? @object) => @object is ManagedEvent other && Equals(other);

    public static bool operator ==(ManagedEvent? left, ManagedEvent? right) =>
        (object?)left == null ? (object?)right == null : left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ManagedEvent? left, ManagedEvent? right) => !(left == right);
}