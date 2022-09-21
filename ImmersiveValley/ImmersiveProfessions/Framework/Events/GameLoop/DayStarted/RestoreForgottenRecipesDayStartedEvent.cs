namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Common.Events;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Collections;
using DaLion.Common.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RestoreForgottenRecipesDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RestoreForgottenRecipesDayStartedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = Game1.player.Read("ForgottenRecipesDict")
            .ParseDictionary<string, int>();
        if (forgottenRecipes.Count <= 0)
        {
            this.Disable();
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

        Game1.player.Write(
            "ForgottenRecipesDict",
            forgottenRecipes.Count > 0 ? forgottenRecipes.Stringify() : null);
        this.Disable();
    }
}
