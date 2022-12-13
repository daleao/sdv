namespace DaLion.Overhaul.Modules.Professions.Ultimates;

/// <summary>Interface for Ultimate abilities.</summary>
public interface IUltimate
{
    /// <summary>Gets the corresponding combat profession.</summary>
    IProfession Profession { get; }

    /// <summary>Gets the localized and gendered name for this <see cref="IUltimate"/>.</summary>
    string DisplayName { get; }

    /// <summary>Gets get the localized description text for this <see cref="IUltimate"/>.</summary>
    string Description { get; }

    /// <summary>Gets the index of the <see cref="IUltimate"/>, which equals the index of the corresponding combat profession.</summary>
    int Index { get; }

    /// <summary>Gets a value indicating whether whether the <see cref="IUltimate"/> is currently active.</summary>
    bool IsActive { get; }

    /// <summary>Gets or sets the current charge value.</summary>
    double ChargeValue { get; set; }

    /// <summary>Gets the maximum charge value.</summary>
    int MaxValue { get; }

    /// <summary>Gets a value indicating whether whether all activation conditions for the <see cref="IUltimate"/> are currently met.</summary>
    bool CanActivate { get; }

    /// <summary>Gets a value indicating whether whether the <see cref="UltimateHud"/> is currently rendering.</summary>
    bool IsHudVisible { get; }
}
