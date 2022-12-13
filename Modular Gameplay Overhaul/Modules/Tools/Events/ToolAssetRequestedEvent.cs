namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using System.Globalization;
using DaLion.Shared.Content;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ToolAssetRequestedEvent : AssetRequestedEvent
{
    private const int MinDamage = 2;
    private const int MaxDamage = 3;
    private const int Knockback = 4;
    private const int Aoe = 11;
    private const int CritChance = 12;
    private const int CritPower = 13;

    /// <summary>Initializes a new instance of the <see cref="ToolAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Data/weapons", new AssetEditor(EditToolsData, AssetEditPriority.Late));
    }

    #region editor callbacks

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditToolsData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');
            switch (key)
            {
                case Constants.ScytheIndex: // scythe
                    fields[Aoe] = (ToolsModule.Config.Scythe.RegularRadius * Game1.tileSize).ToString();

                    if (Config.EnableArsenal && ArsenalModule.Config.Weapons.RebalancedWeapons)
                    {
                        fields[MinDamage] = 1.ToString();
                        fields[MaxDamage] = 1.ToString();
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                case Constants.GoldenScytheIndex: // golden scythe
                    fields[Aoe] = (ToolsModule.Config.Scythe.GoldRadius * Game1.tileSize).ToString();

                    if (Config.EnableArsenal && ArsenalModule.Config.Weapons.RebalancedWeapons)
                    {
                        fields[MinDamage] = 13.ToString();
                        fields[MaxDamage] = 13.ToString();
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 2.0.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
            }

            data[key] = string.Join('/', fields);
        }
    }

    #endregion editor callbacks
}
