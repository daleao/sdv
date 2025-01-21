namespace DaLion.Core.Framework;

#region using directives

using System.Diagnostics.CodeAnalysis;
using StardewValley.Monsters;

#endregion using directives

/// <summary>A wrapper for fake <see cref="Farmer"/> instances used for faking typically farmer-only interactions.</summary>
public sealed class FakeFarmer : Farmer
{
    /// <summary>Gets or sets the targeted enemy <see cref="Monster"/>.</summary>
    public Monster? AttachedEnemy { get; set; }

    /// <summary>Gets a value indicating whether the <see cref="FakeFarmer"/> is target enemy <see cref="Monster"/>.</summary>
    [MemberNotNullWhen(true, "AttachedEnemy")]
    public bool IsAttachedToEnemy => this.AttachedEnemy is not null;
}
