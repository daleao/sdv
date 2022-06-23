namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Common;
using Common.Events;
using Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSavingEvent : SavingEvent
{
    /// <inheritdoc />
    protected override void OnSavingImpl(object sender, SavingEventArgs e)
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

                if (!Enum.TryParse<ModData>(split[2], out var field))
                {
                    data.Remove(key);
                    ++count;
                    continue;
                }

                if (!Profession.TryFromName(field.ToString().SplitCamelCase()[0], out var profession) ||
                    Game1.player.HasProfession(profession)) continue;

                data.Remove(key);
                ++count;
            }

        Log.D($"[ModData]: Found {count} rogue data fields.");
    }
}