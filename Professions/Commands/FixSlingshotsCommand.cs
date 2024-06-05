namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FixSlingshotsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class FixSlingshotsCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["fix_slingshots"];

    /// <inheritdoc />
    public override string Documentation =>
        "Sets the ammo count for every slingshot in the player's inventory according to whether or not they have the Rascal profession.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var player = Game1.player;
        for (var i = 0; i < player.Items.Count; i++)
        {
            var item = player.Items[i];
            if (item is not Slingshot slingshot)
            {
                continue;
            }

            Slingshot? replacement = null;
            if (player.HasProfession(Profession.Rascal) &&
                (slingshot.AttachmentSlotsCount != 2 || slingshot.attachments.Length != 2))
            {
                replacement = ItemRegistry.Create<Slingshot>(slingshot.QualifiedItemId);
                replacement.AttachmentSlotsCount = 2;
                if (slingshot.attachments[0] is { } ammo)
                {
                    replacement.attachments[0] = (SObject)ammo.getOne();
                    replacement.attachments[0].Stack = ammo.Stack;
                }
            }
            else if (!player.HasProfession(Profession.Rascal) &&
                     (slingshot.AttachmentSlotsCount == 2 || slingshot.attachments.Length == 2))
            {
                replacement = ItemRegistry.Create<Slingshot>(slingshot.QualifiedItemId);
                if (slingshot.attachments[0] is { } ammo1)
                {
                    replacement.attachments[0] = (SObject)ammo1.getOne();
                    replacement.attachments[0].Stack = ammo1.Stack;
                }

                if (slingshot.attachments.Length > 1 && slingshot.attachments[1] is { } ammo2)
                {
                    var drop = (SObject)ammo2.getOne();
                    drop.Stack = ammo2.Stack;
                    if (!player.addItemToInventoryBool(drop))
                    {
                        Game1.createItemDebris(drop, player.getStandingPosition(), -1, player.currentLocation);
                    }
                }
            }

            if (replacement is null)
            {
                continue;
            }

            for (var j = slingshot.enchantments.Count - 1; j >= 0; j--)
            {
                var enchantment = slingshot.enchantments[j];
                replacement.AddEnchantment(enchantment);
                slingshot.RemoveEnchantment(enchantment);
            }

            player.Items[i] = replacement;
        }

        return true;
    }
}
