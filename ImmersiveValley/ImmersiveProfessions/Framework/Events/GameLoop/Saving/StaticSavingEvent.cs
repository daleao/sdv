namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common;
using Common.Events;
using Common.Extensions;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSavingEvent : SavingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticSavingEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysHooked = true;
    }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        // clean rogue data
        Log.D("[ModData]: Checking for rogue data fields...");
        var data = Game1.player.modData;
        var count = 0;
        if (!Context.IsMainPlayer)
            for (var i = data.Keys.Count() - 1; i >= 0; --i)
            {
                var key = data.Keys.ElementAt(i);
                if (!key.StartsWith(ModEntry.Manifest.UniqueID)) continue;

                data.Remove(key);
                ++count;
            }
        else
            for (var i = data.Keys.Count() - 1; i >= 0; --i)
            {
                var key = data.Keys.ElementAt(i);
                if (!key.StartsWith(ModEntry.Manifest.UniqueID)) continue;

                var split = key.Split('/');
                if (split.Length != 3 || !split[1].TryParse<long>(out var id))
                {
                    data.Remove(key);
                    ++count;
                    continue;
                }

                var who = Game1.getFarmerMaybeOffline(id);
                if (who is null)
                {
                    data.Remove(key);
                    ++count;
                    continue;
                }

                var allFields = new[]
                {
                    "EcologistItemsForaged", "GemologistMineralsCollected", "ProspectorHuntStreak",
                    "ScavengerHuntStreak", "ConservationistTrashCollectedThisSeason",
                    "ConservationistActiveTaxBonusPct", "ForgottenRecipesDict", "UltimateIndex"
                };
                var field = split[2];
                if (!field.IsIn(allFields))
                {
                    data.Remove(key);
                    ++count;
                    continue;
                }

                if (!Profession.TryFromName(field.SplitCamelCase()[0], out var profession) ||
                    Game1.player.HasProfession(profession)) continue;

                data.Remove(key);
                ++count;
            }

        Log.D($"[ModData]: Removed {count} rogue data fields.");
    }
}