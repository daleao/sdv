namespace DaLion.Shared.Classes;

/// <summary>Base class for the singleton design pattern.</summary>
/// <typeparam name="TInstance">The <see langword="class"/> type of the singleton instance.</typeparam>
public sealed class Singleton<TInstance>
    where TInstance : class
{
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<TInstance?> _instance = new(CreateInstance);

    /// <summary>Initializes a new instance of the <see cref="Singleton{T}"/> class.</summary>
    private Singleton()
    {
    }

    /// <summary>Gets the singleton instance for this class.</summary>
    public static TInstance? Instance => _instance.Value;

    /// <summary>Gets a value indicating whether an instance has been created for the singleton class.</summary>
    public static bool IsValueCreated => _instance.IsValueCreated && _instance.Value is not null;

    private static TInstance? CreateInstance()
    {
        return (TInstance?)Activator.CreateInstance(typeof(TInstance), nonPublic: true);
    }
}
