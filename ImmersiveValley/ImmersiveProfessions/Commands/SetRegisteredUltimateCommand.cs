namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewValley;

using Common;
using Common.Commands;
using Common.Data;
using Common.Extensions;
using Extensions;
using Framework;
using Framework.Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class SetRegisteredUltimateCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetRegisteredUltimateCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "set_ult";

    /// <inheritdoc />
    public override string Documentation => "Change the player's currently registered Special Ability.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!args.Any() || args.Length > 1)
        {
            Log.W("You must specify a single value.");
            return;
        }

        if (!Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            Log.W("You don't have any 2nd-tier combat professions.");
            return;
        }

        args[0] = args[0].ToLowerInvariant().FirstCharToUpper();
        if (!Enum.TryParse<UltimateIndex>(args[0], true, out var index))
        {
            Log.W("You must enter a valid 2nd-tier combat profession.");
            return;
        }

        var profession = Profession.FromValue((int) index);
        if (!Game1.player.HasProfession(profession))
        {
            Log.W("You don't have this profession.");
            return;
        }

#pragma warning disable CS8509
        ModEntry.PlayerState.RegisteredUltimate = index switch
#pragma warning restore CS8509
        {
            UltimateIndex.BruteFrenzy => new UndyingFrenzy(),
            UltimateIndex.PoacherAmbush => new Ambush(),
            UltimateIndex.PiperPandemic => new Enthrall(),
            UltimateIndex.DesperadoBlossom => new DeathBlossom()
        };

        ModDataIO.WriteData(Game1.player, ModData.UltimateIndex.ToString(), index.ToString());
    }
}