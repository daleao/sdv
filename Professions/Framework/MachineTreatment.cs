namespace DaLion.Professions.Framework;

/// <summary>A category of treatment that can be applied to an artisan machine.</summary>
internal readonly struct MachineTreatment
{
    /// <summary>Initializes a new instance of the <see cref="MachineTreatment"/> struct.</summary>
    /// <param name="id">A string ID for this treatment.</param>
    /// <param name="displayName">A display name this treatment.</param>
    internal MachineTreatment(string id, string displayName)
    {
        this.Id = id;
        this.DisplayName = displayName;
    }

    /// <summary>Gets a string ID for this treatment.</summary>
    internal string Id { get; }

    /// <summary>Gets an in-game display name for this treatment.</summary>
    internal string DisplayName { get; }

    /// <summary>Compares two <see cref="MachineTreatment"/>s.</summary>
    /// <param name="left">Source <see cref="MachineTreatment" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="MachineTreatment" /> on the right of the add sign.</param>
    /// <returns><see langword="true"/> if both treatment IDs are equal, otherwise <see langword="false"/>.</returns>
    public static bool operator ==(MachineTreatment left, MachineTreatment right)
    {
        return left.Id == right.Id;
    }

    /// <summary>Compares two <see cref="MachineTreatment"/>s.</summary>
    /// <param name="left">Source <see cref="MachineTreatment" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="MachineTreatment" /> on the right of the add sign.</param>
    /// <returns><see langword="true"/> if both treatment IDs are different, otherwise <see langword="false"/>.</returns>
    public static bool operator !=(MachineTreatment left, MachineTreatment right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    public override bool Equals(object? @object)
    {
        return @object is MachineTreatment treatment && this == treatment;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (this.Id + "MachineTreatment").GetHashCode();
    }

    /// <inheritdoc/>
    public override readonly string ToString() => this.Id;
}
