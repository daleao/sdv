namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Extensions;

#endregion using directives

internal class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = Game1.player.ReadData(DataField.ForgottenRecipesDict).ToDictionary<string, int>(",", ";");
        if (!forgottenRecipes.Any())
        {
            Disable();
            return;
        }

        for (var i = forgottenRecipes.Count - 1; i >= 0; --i)
        {
            var key = forgottenRecipes.ElementAt(i).Key;
            if (Game1.player.craftingRecipes.ContainsKey(key))
            {
                Game1.player.craftingRecipes[key] += forgottenRecipes[key];
                forgottenRecipes.Remove(key);
            }
            else if (Game1.player.cookingRecipes.ContainsKey(key))
            {
                Game1.player.cookingRecipes[key] += forgottenRecipes[key];
                forgottenRecipes.Remove(key);
            }
        }

        Game1.player.WriteData(DataField.ForgottenRecipesDict, forgottenRecipes.Any()
            ? forgottenRecipes.ToString(",", ";")
            : null);
        Disable();
    }
}