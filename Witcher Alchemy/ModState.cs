namespace DaLion.Alchemy;

#region using directives



#endregion using directives

/// <summary>The ephemeral mod state.</summary>
internal sealed class ModState
{
    internal int Toxicity { get; set; }

    internal bool UsingGridView = false;
    
    internal bool AppliedFiltering = false;
    
    internal bool ReversedSortOrder = false;
}
