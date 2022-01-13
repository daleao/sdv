using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using DaLion.Stardew.Common.Extensions;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

internal class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = ModData.Read(DataField.ForgottenRecipesDict).ToDictionary<string, int>(",", ";");
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

        ModData.Write(DataField.ForgottenRecipesDict, forgottenRecipes.Any()
            ? forgottenRecipes.ToString(",", ";")
            : null);
        Disable();
    }
}