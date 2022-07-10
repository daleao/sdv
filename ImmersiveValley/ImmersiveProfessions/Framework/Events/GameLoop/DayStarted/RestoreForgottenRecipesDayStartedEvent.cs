namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Data;
using Common.Events;
using Common.Extensions;
using Common.Extensions.Collections;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class RestoreForgottenRecipesDayStartedEvent : DayStartedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RestoreForgottenRecipesDayStartedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        var forgottenRecipes = ModDataIO.ReadFrom(Game1.player, "ForgottenRecipesDict")
            .ParseDictionary<string, int>();
        if (forgottenRecipes.Count <= 0)
        {
            Unhook();
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

        ModDataIO.WriteTo(Game1.player, "ForgottenRecipesDict", forgottenRecipes.Count > 0
            ? forgottenRecipes.Stringify()
            : null);
        Unhook();
    }
}