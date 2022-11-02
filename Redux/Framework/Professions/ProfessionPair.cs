namespace DaLion.Redux.Framework.Professions;

#region using directives

using System.Collections;
using System.Collections.Generic;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Represents a pair of profession choices offered to the player during level-up.</summary>
/// <param name="First">The first profession in the pair.</param>
/// <param name="Second">The second profession in the pair.</param>
/// <param name="Requires">The level 5 profession from which this pair branches out of, if this is a level 10 pair.</param>
/// <param name="Level">Either <c>5</c> or <c>10</c>.</param>
public record ProfessionPair
    (IProfession First, IProfession Second, IProfession? Requires, int Level) : IEnumerable<IProfession>
{
    /// <inheritdoc />
    public IEnumerator<IProfession> GetEnumerator()
    {
        return this.First.Collect(this.Second).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
