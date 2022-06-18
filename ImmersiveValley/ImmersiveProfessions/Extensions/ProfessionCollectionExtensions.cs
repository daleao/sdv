namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewValley;

using Framework;

#endregion using directives

public static class ProfessionCollectionExtensions
{
    public static IEnumerable<IProfession> AcquiredByFarmer(this ICollection<IProfession> professions, Farmer farmer)
    {
        return professions.Where(p => farmer.professions.Contains(p.Id)).AsEnumerable();
    }
}