namespace DaLion.Ligo.Modules.Tools.Events;

#region using directives

using System.Collections.Generic;
using System.Globalization;
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

    private const int ScytheIndex = 47;
    private const int GoldenScytheIndex = 53;

    private static readonly Dictionary<string, (Action<IAssetData> Callback, AssetEditPriority Priority)> AssetEditors =
        new();

    /// <summary>Initializes a new instance of the <see cref="ToolAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        AssetEditors["Data/weapons"] = (Callback: EditToolsData, Priority: AssetEditPriority.Late);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
        {
            e.Edit(editor.Callback, editor.Priority);
        }
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
                case ScytheIndex: // scythe
                    fields[Aoe] = ModEntry.Config.Tools.Scythe.RegularRadius.ToString();

                    if (ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.Weapons.RebalanceWeapons)
                    {
                        fields[MinDamage] = 1.ToString();
                        fields[MaxDamage] = 1.ToString();
                        fields[Knockback] = 0.5.ToString(CultureInfo.InvariantCulture);
                        fields[CritChance] = 0.125.ToString(CultureInfo.InvariantCulture);
                        fields[CritPower] = 1.5.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
                case GoldenScytheIndex: // golden scythe
                    fields[Aoe] = ModEntry.Config.Tools.Scythe.GoldRadius.ToString();

                    if (ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.Weapons.RebalanceWeapons)
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
