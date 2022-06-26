namespace DaLion.Common.Events;

#region using directives

using System;
using System.Runtime.CompilerServices;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation of an event wrapper allowing dynamic hooking / unhooking.</summary>
internal abstract class ManagedEvent : IManagedEvent, IEquatable<ManagedEvent>
{
    /// <summary>The <see cref="EventManager"/> instance that manages this event.</summary>
    protected EventManager Manager { get; init; }

    /// <summary>Whether this event is hooked on each screen.</summary>
    protected readonly PerScreen<bool> Hooked = new();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ManagedEvent(EventManager manager)
    {
        Manager = manager;
    }

    /// <inheritdoc />
    public bool IsHooked => Hooked.Value;

    /// <inheritdoc />
    public bool IsHookedForScreen(int screenID)
    {
        return Hooked.GetValueForScreen(screenID);
    }

    /// <inheritdoc />
    public void Hook()
    {
        Hooked.Value = true;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        Hooked.Value = false;
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