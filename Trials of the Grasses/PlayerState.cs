namespace DaLion.Stardew.Alchemy;

#region using directives

using Framework;
using Framework.UI;
using System.Collections.Generic;

#endregion using directives

internal class PlayerState
{
    internal HashSet<Formula>? KnownFormulae;
    internal int CauldronLevel;

    internal bool UsingGridView = false;
    internal bool AppliedFiltering = false;
    internal bool ReversedSortOrder = false;
    internal AlchemyMenu.Autofill Autofill = AlchemyMenu.Autofill.Off;
}