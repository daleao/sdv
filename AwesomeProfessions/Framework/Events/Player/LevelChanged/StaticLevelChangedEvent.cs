namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal class StaticLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object sender, LevelChangedEventArgs e)
    {
        if (e.Skill == SkillType.Combat) LevelUpMenu.RevalidateHealth(Game1.player);
    }
}