namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Commands;
using DaLion.Stardew.Arsenal.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[DebugOnly]
internal sealed class PurifySwordCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PurifySwordCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PurifySwordCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "purify" };

    /// <inheritdoc />
    public override string Documentation => "Transform a currently held Dark Sword into a Holy Blade.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex })
        {
            Game1.player.CurrentTool = new MeleeWeapon(Constants.DarkSwordIndex);
        }

        Game1.player.Halt();
        Game1.player.faceDirection(2);
        Game1.player.showCarrying();
        Game1.player.jitterStrength = 1f;
        Game1.pauseThenDoFunction(3000, FarmerExtensions.GetHolyBlade);
        Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
        Game1.currentLocation.playSound("crit");
        Game1.screenGlowOnce(Color.Transparent, true, 0.01f, 0.999f);
        DelayedAction.playSoundAfterDelay("stardrop", 1500);
        Game1.screenOverlayTempSprites.AddRange(
            Utility.sparkleWithinArea(
                new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                500,
                Color.Gold,
                10,
                2000));
        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(
            Game1.afterDialogues, (Game1.afterFadeFunction)(() => Game1.stopMusicTrack(Game1.MusicContext.Event)));
    }
}
