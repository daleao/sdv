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

    /// <inheritdoc/>
    public override readonly string ToString() => this.Id;
}
