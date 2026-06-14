namespace DaLion.Professions.Framework;

internal enum MachineTreatmentCategory
{
    /// <summary>Conditions the vessel for cultured aging and fermentation using Oak Resin.</summary>
    Fermentation,

    /// <summary>Seasons the processing medium with a sweet glaze using Maple Syrup or Birch Water (SVE).</summary>
    Glazing,

    /// <summary>Seals the processing environment for smoking, drying, and extraction using Pine Tar or Fir Wax (SVE).</summary>
    Sealing,

    /// <summary>Medium cannot be treated.</summary>
    None = -1,
}
