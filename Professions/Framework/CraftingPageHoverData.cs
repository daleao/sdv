namespace DaLion.Professions.Framework;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

internal record CraftingPageHoverData(CraftingRecipe? HoverRecipe)
{
    internal static int SelectedIngredientIndex { get; private set; }

    internal void ScrollUp()
    {
        var newValue = Math.Max(SelectedIngredientIndex - 1, 0);
        if (SelectedIngredientIndex != newValue)
        {
            SelectedIngredientIndex = newValue;
            Game1.playSound("bigSelect");
        }
    }

    internal void ScrollDown()
    {
        var newValue = Math.Min(SelectedIngredientIndex + 1, this.HoverRecipe!.recipeList.Count - 1);
        if (SelectedIngredientIndex != newValue)
        {
            SelectedIngredientIndex = newValue;
            Game1.playSound("bigSelect");
        }
    }
}
