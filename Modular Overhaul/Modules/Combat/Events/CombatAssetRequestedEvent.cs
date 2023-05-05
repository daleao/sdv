namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Strings/StringsFromCSFiles", new AssetEditor(EditStringsFromCsFiles, AssetEditPriority.Late));
    }

    #region editor callbacks

    /// <summary>Adjust Jinxed debuff description.</summary>
    private static void EditStringsFromCsFiles(IAssetData asset)
    {
        if (!CombatModule.Config.OverhauledDefense)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Buff.cs.465"] = I18n.Ui_Buffs_Jinxed();
    }

    #endregion editor callbacks
}
