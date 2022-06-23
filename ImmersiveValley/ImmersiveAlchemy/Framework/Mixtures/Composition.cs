namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using Enums;

#endregion using directives

/// <summary>Represents an ingredient's alchemical makeup.</summary>
/// <param name="Primary">The <see cref="PrimarySubstance"/> contained in the ingredient.</param>
/// <param name="Density">The <see cref="SubstanceDensity"/> of <see cref="PrimarySubstance"/>.</param>
/// <param name="Secondary">The <see cref="SecondarySubstance"/> contained in the ingredient, if any.</param>
public record Composition(PrimarySubstance Primary, SubstanceDensity Density, SecondarySubstance? Secondary);