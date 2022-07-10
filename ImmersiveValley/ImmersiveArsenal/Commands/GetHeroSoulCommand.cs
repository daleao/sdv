namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using Common;
using Common.Commands;
using JetBrains.Annotations;
using StardewValley;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class GetHeroSoulCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal GetHeroSoulCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "hero_soul";

    /// <inheritdoc />
    public override string Documentation => "Add the specified amount of Hero Soul to the local player's inventory.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 1)
            Log.W("Additional arguments beyond the first will be ignored.");

        var stack = 1;
        if (args.Length > 0 && !int.TryParse(args[0], out stack))
            Log.W($"Received invalid value {args[0]} for `stack` parameter. The parameter will be ignored.");

        var heroSoul = (SObject)ModEntry.DynamicGameAssetsApi!.SpawnDGAItem(ModEntry.Manifest.UniqueID + "/Hero Soul");
        heroSoul.Stack = stack;
        Utility.CollectOrDrop(heroSoul);
    }
}