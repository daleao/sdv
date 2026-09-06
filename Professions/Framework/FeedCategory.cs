namespace DaLion.Professions.Framework;

/// <summary>A category of feed that can be given as nutritious supplement to an animal.</summary>
internal readonly struct FeedCategory
{
    /// <summary>Initializes a new instance of the <see cref="FeedCategory"/> struct.</summary>
    /// <param name="id">A string ID for this category.</param>
    internal FeedCategory(string id)
    {
        this.Id = id;
    }

    /// <summary>Gets a string ID for this category.</summary>
    internal string Id { get; }

    /// <summary>Compares two <see cref="FeedCategory"/>s.</summary>
    /// <param name="left">Source <see cref="FeedCategory" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="FeedCategory" /> on the right of the add sign.</param>
    /// <returns><see langword="true"/> if both treatment IDs are equal, otherwise <see langword="false"/>.</returns>
    public static bool operator ==(FeedCategory left, FeedCategory right)
    {
        return left.Id == right.Id;
    }

    /// <summary>Compares two <see cref="FeedCategory"/>s.</summary>
    /// <param name="left">Source <see cref="FeedCategory" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="FeedCategory" /> on the right of the add sign.</param>
    /// <returns><see langword="true"/> if both treatment IDs are different, otherwise <see langword="false"/>.</returns>
    public static bool operator !=(FeedCategory left, FeedCategory right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    public override bool Equals(object? @object)
    {
        return @object is FeedCategory treatment && this == treatment;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (this.Id + "FeedCategory").GetHashCode();
    }

    /// <inheritdoc/>
    public override readonly string ToString() => this.Id;
}
