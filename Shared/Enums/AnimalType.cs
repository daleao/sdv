namespace DaLion.Shared.Enums;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The actual species of <see cref="FarmAnimal"/>.</summary>
[EnumExtensions]
public enum AnimalType
{
    /// <summary>A chicken, regardless of color.</summary>
    Chicken,

    /// <summary>A cow, regardless of color.</summary>
    Cow,

    /// <summary>A sheep.</summary>
    Sheep,

    /// <summary>A goat.</summary>
    Goat,

    /// <summary>A duck.</summary>
    Duck,

    /// <summary>A rabbit.</summary>
    Rabbit,

    /// <summary>A pig.</summary>
    Pig,

    /// <summary>An ostrich.</summary>
    Ostrich,

    /// <summary>A dinosaur.</summary>
    Dinosaur,

    /// <summary>A bear, added by SVE.</summary>
    Bear,

    /// <summary>A Camel, added by SVE.</summary>
    Camel,
}
