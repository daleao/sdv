namespace DaLion.Common.Events;

#region using directives

using System;
using System.Runtime.CompilerServices;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation of an event wrapper allowing dynamic hooking / unhooking.</summary>
internal abstract class ManagedEvent : IManagedEvent, IEquatable<ManagedEvent>
{
    /// <summary>Whether this event is hooked on each screen.</summary>
    private readonly PerScreen<bool> _Hooked = new();

    /// <summary>The <see cref="EventManager"/> instance that manages this event.</summary>
    protected EventManager Manager { get; init; }
    
    /// <summary>Allow this event to be raised even when unhooked.</summary>
    protected bool AlwaysHooked { get; init; } = false;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ManagedEvent(EventManager manager)
    {
        Manager = manager;
    }

    /// <inheritdoc />
    public bool IsHooked => _Hooked.Value || AlwaysHooked;

    /// <inheritdoc />
    public bool IsHookedForScreen(int screenID)
    {
        return _Hooked.GetValueForScreen(screenID);
    }

    /// <inheritdoc />
    public void Hook()
    {
        _Hooked.Value = true;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        _Hooked.Value = false;
    }

    /// <inheritdoc />
    public override string ToString() => GetType().Name;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => GetType().GetHashCode();

    /// <summary>Determines if the specified <see cref="ManagedEvent"/> is equal to the current instance.</summary>
    /// <param name="other">A <see cref="ManagedEvent"/> to compare to this instance.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> has the same type as this instance; otherwise, <see langword="false">.</returns>
    public virtual bool Equals(ManagedEvent? other)
    {
        // ReSharper disable once CheckForReferenceEqualityInstead.1
        return GetType().Equals(other?.GetType());
    }

    /// <inheritdoc />
    public override bool Equals(object? @object)
    {
        return @object is ManagedEvent other && Equals(other);
    }

    public static bool operator ==(ManagedEvent? left, ManagedEvent? right) =>
        (object?) left == null ? (object?) right == null : left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ManagedEvent? left, ManagedEvent? right) => !(left == right);
}