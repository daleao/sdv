namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Globalization;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalAssetRequestedEvent : AssetRequestedEvent
{
    private const int
        MIN_DAMAGE_I = 2,
        MAX_DAMAGE_I = 3,
        KNOCKBACK_I = 4,
        SPEED_I = 5,
        PRECISION_I = 6,
        DEFENSE_I = 7,
        TYPE_I = 8,
        BASE_DROP_LEVEL_I = 9,
        MIN_DROP_LEVEL_I = 10,
        AOE_I = 11,
        CRIT_CHANCE_I = 12,
        CRIT_POWER_I = 13;

    private static readonly Dictionary<string, (Action<IAssetData> callback, AssetEditPriority priority)> AssetEditors =
        new();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        //AssetEditors["Data/Boots"] = (callback: DataBootsEditor, priority: AssetEditPriority.Default);
        AssetEditors["Data/weapons"] = (callback: EditWeaponsData, priority: AssetEditPriority.Late);
        AssetEditors["Data/Quests"] = (callback: EditQuestsData, priority: AssetEditPriority.Default);
        AssetEditors["Data/mail"] = (callback: EditMailData, priority: AssetEditPriority.Default);
        AssetEditors["Strings/Locations"] = (callback: EditLocationsStrings, priority: AssetEditPriority.Default);
        AssetEditors["Strings/StringsFromCSFiles"] =
            (callback: EditStringsFromCSFilesStrings, priority: AssetEditPriority.Default);
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (AssetEditors.TryGetValue(e.NameWithoutLocale.Name, out var editor))
            e.Edit(editor.callback, editor.priority);
    }

    #region editor callbacks

    /// <summary>Edits boots data with rebalanced stats.</summary>
    private static void EditBootsData(IAssetData asset)
    {
        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');
            switch (key)
            {
                #region footwear switch-case

                case 504: // sneakers
                    break;
                case 505: // rubber boots
                    break;
                case 506: // leather boots
                    break;
                case 507: // work boots
                    break;
                case 508: // combat boots
                    break;
                case 509: // tundra boots
                    break;
                case 510: // thermal boots
                    break;
                case 511: // dark boots
                    break;
                case 512: // firewalker boots
                    break;
                case 513: // genie shoes
                    break;
                case 514: // space boots
                    break;
                case 515: // cowboy boots
                    break;
                case 804: // emily's magic boots
                    break;
                case 806: // leprechaun shoes
                    break;
                case 853: // cinderclown shoes
                    break;
                case 854: // mermaid boots
                    break;
                case 855: // dragonscale boots
                    break;
                case 878: // crystal shoes
                    break;

                    #endregion footwear switch-case
            }

            data[key] = string.Join('/', fields);
        }
    }

    /// <summary>Edits quest data with custom legendary sword quest.</summary>
    private static void EditQuestsData(IAssetData asset)
    {
        if (!ModEntry.Config.TrulyLegendaryGalaxySword) return;

        var data = asset.AsDictionary<int, string>().Data;
        var title = ModEntry.i18n.Get("quests.QiChallengeFinal.title");
        var description = ModEntry.i18n.Get("quests.QiChallengeFinal.desc");
        var objective = ModEntry.i18n.Get("quests.QiChallengeFinal.obj");
        data[ModEntry.QiChallengeFinalQuestId] = $"Basic/{title}/{description}/{objective}/-1/-1/0/-1/false";
    }

    /// <summary>Edits mail data with custom legendary sword quest mail.</summary>
    private static void EditMailData(IAssetData asset)
    {
        if (!ModEntry.Config.TrulyLegendaryGalaxySword) return;

        var data = asset.AsDictionary<string, string>().Data;
        data["MeleeWeapon"] = ModEntry.i18n.Get("mail.skullCave");
        data["QiChallengeFirst"] = ModEntry.i18n.Get("mail.QiChallengeFirst", new { ModEntry.QiChallengeFinalQuestId });
        data["QiChallengeComplete"] = ModEntry.i18n.Get("mail.QiChallengeComplete");
    }

    /// <summary>Edits weapons data with rebalanced stats.</summary>
    private static void EditWeaponsData(IAssetData asset)
    {
        if (!ModEntry.Config.RebalancedWeapons) return;

        var data = asset.AsDictionary<int, string>().Data;
        var keys = data.Keys;
        foreach (var key in keys)
        {
            var fields = data[key].Split('/');
            switch (key)
            {
                #region weapon switch-case

                // STABBING SWORDS
                case 12: // wooden blade
                    fields[MIN_DAMAGE_I] = 2.ToString();
                    fields[MAX_DAMAGE_I] = 5.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 2.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[AOE_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.2.ToString(CultureInfo.InvariantCulture);
                    break;
                case 11: // steel smallsword
                    fields[MIN_DAMAGE_I] = 4.ToString();
                    fields[MAX_DAMAGE_I] = 8.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 0.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = 5.ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 44: // cutlass
                    fields[MIN_DAMAGE_I] = 9.ToString();
                    fields[MAX_DAMAGE_I] = 17.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 0.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = 20.ToString();
                    fields[MIN_DROP_LEVEL_I] = 5.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 49: // rapier
                    fields[MIN_DAMAGE_I] = 15.ToString();
                    fields[MAX_DAMAGE_I] = 25.ToString();
                    fields[KNOCKBACK_I] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 8.ToString();
                    fields[PRECISION_I] = 8.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = 60.ToString();
                    fields[MIN_DROP_LEVEL_I] = 35.ToString();
                    fields[AOE_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.35.ToString(CultureInfo.InvariantCulture);
                    break;
                case 50: // steel falchion
                    fields[MIN_DAMAGE_I] = 30.ToString();
                    fields[MAX_DAMAGE_I] = 45.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 4.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = 80.ToString();
                    fields[MIN_DROP_LEVEL_I] = 55.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.75.ToString(CultureInfo.InvariantCulture);
                    break;
                case 43: // pirate's sword
                    fields[MIN_DAMAGE_I] = 37.ToString();
                    fields[MAX_DAMAGE_I] = 55.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 4.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 15: // forest sword
                    fields[MIN_DAMAGE_I] = 42.ToString();
                    fields[MAX_DAMAGE_I] = 60.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 2.ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[AOE_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 5: // bone sword
                    fields[MIN_DAMAGE_I] = 35.ToString();
                    fields[MAX_DAMAGE_I] = 53.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 4.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.4.ToString(CultureInfo.InvariantCulture);
                    break;
                case 13: // insect head
                    fields[MIN_DAMAGE_I] = 1.ToString();
                    fields[MAX_DAMAGE_I] = 50.ToString();
                    fields[KNOCKBACK_I] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[PRECISION_I] = 12.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_CHANCE_I] = 0.15.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.333.ToString(CultureInfo.InvariantCulture);
                    break;
                case 2: // dark sword
                    fields[MIN_DAMAGE_I] = 50.ToString();
                    fields[MAX_DAMAGE_I] = 65.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 0.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 8: // obsidian edge
                    fields[MIN_DAMAGE_I] = 56.ToString();
                    fields[MAX_DAMAGE_I] = 64.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 0.ToString();
                    fields[PRECISION_I] = 4.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = 135.ToString();
                    fields[MIN_DROP_LEVEL_I] = 100.ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 9: // lava katana
                    fields[MIN_DAMAGE_I] = 75.ToString();
                    fields[MAX_DAMAGE_I] = 90.ToString();
                    fields[KNOCKBACK_I] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 2.ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.06.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 4: // galaxy sword
                    fields[MIN_DAMAGE_I] = 80.ToString();
                    fields[MAX_DAMAGE_I] = 95.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 2.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 2.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[AOE_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 57: // dragontooth cutlass
                    fields[MIN_DAMAGE_I] = 145.ToString();
                    fields[MAX_DAMAGE_I] = 175.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 0.ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 62: // infinity blade
                    fields[MIN_DAMAGE_I] = 140.ToString();
                    fields[MAX_DAMAGE_I] = 160.ToString();
                    fields[KNOCKBACK_I] = 1.ToString();
                    fields[SPEED_I] = 2.ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[DEFENSE_I] = 0.ToString();
                    fields[TYPE_I] = 0.ToString();
                    fields[AOE_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.05.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;

                // DAGGERS
                case 16: // carving knife
                    fields[BASE_DROP_LEVEL_I] = 1.ToString();
                    fields[CRIT_CHANCE_I] = 0.1.ToString(CultureInfo.InvariantCulture);
                    break;
                case 22: // wind spire
                    fields[MIN_DAMAGE_I] = 2.ToString();
                    fields[KNOCKBACK_I] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 1.ToString();
                    fields[CRIT_CHANCE_I] = 0.11.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 3.2.ToString(CultureInfo.InvariantCulture);
                    break;
                case 17: // iron dirk
                    fields[MIN_DAMAGE_I] = 5.ToString();
                    fields[MAX_DAMAGE_I] = 8.ToString();
                    fields[BASE_DROP_LEVEL_I] = 25.ToString();
                    fields[MIN_DROP_LEVEL_I] = 10.ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 18: // burglar's shank
                    fields[KNOCKBACK_I] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 1.ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[BASE_DROP_LEVEL_I] = 45.ToString();
                    fields[MIN_DROP_LEVEL_I] = 20.ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 4.ToString();
                    break;
                case 21: // crystal dagger
                    fields[MIN_DAMAGE_I] = 16.ToString();
                    fields[MAX_DAMAGE_I] = 20.ToString();
                    fields[KNOCKBACK_I] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[PRECISION_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.1.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 3.ToString();
                    break;
                case 20: // elf blade
                    fields[MIN_DAMAGE_I] = 13.ToString();
                    fields[MAX_DAMAGE_I] = 18.ToString();
                    fields[KNOCKBACK_I] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 2.ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.12.ToString(CultureInfo.InvariantCulture);
                    break;
                case 19: // shadow blade
                    fields[MIN_DAMAGE_I] = 24.ToString();
                    fields[MAX_DAMAGE_I] = 28.ToString();
                    fields[KNOCKBACK_I] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 1.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[AOE_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 51: // broken trident
                    fields[MIN_DAMAGE_I] = 16.ToString();
                    fields[MAX_DAMAGE_I] = 22.ToString();
                    fields[KNOCKBACK_I] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[PRECISION_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.06.ToString(CultureInfo.InvariantCulture);
                    break;
                case 45: // wicked kriss
                    fields[MIN_DAMAGE_I] = 28.ToString();
                    fields[MAX_DAMAGE_I] = 32.ToString();
                    fields[KNOCKBACK_I] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[PRECISION_I] = 6.ToString();
                    fields[BASE_DROP_LEVEL_I] = 110.ToString();
                    fields[MIN_DROP_LEVEL_I] = 85.ToString();
                    fields[AOE_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.2.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 4.ToString();
                    break;
                case 61: // iridium needle
                    fields[MIN_DAMAGE_I] = 31.ToString();
                    fields[MAX_DAMAGE_I] = 33.ToString();
                    fields[KNOCKBACK_I] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 4.ToString();
                    fields[PRECISION_I] = 10.ToString();
                    fields[DEFENSE_I] = (-2).ToString();
                    fields[CRIT_CHANCE_I] = 0.333.ToString(CultureInfo.InvariantCulture);
                    break;
                case 23: // galaxy dagger
                    fields[MIN_DAMAGE_I] = 40.ToString();
                    fields[MAX_DAMAGE_I] = 45.ToString();
                    fields[KNOCKBACK_I] = 0.5.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 3.ToString();
                    fields[AOE_I] = 1.ToString();
                    fields[CRIT_CHANCE_I] = 0.1.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 4.ToString();
                    break;
                case 56: // dwarf dagger
                    fields[MIN_DAMAGE_I] = 60.ToString();
                    fields[MAX_DAMAGE_I] = 65.ToString();
                    fields[SPEED_I] = (-2).ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 59: // dragontooth shiv
                    fields[MIN_DAMAGE_I] = 78.ToString();
                    fields[MAX_DAMAGE_I] = 83.ToString();
                    fields[KNOCKBACK_I] = 0.8.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_CHANCE_I] = 0.1.ToString(CultureInfo.InvariantCulture);
                    break;
                case 64: // infinity dagger
                    fields[MIN_DAMAGE_I] = 75.ToString();
                    fields[MAX_DAMAGE_I] = 80.ToString();
                    fields[KNOCKBACK_I] = 0.6.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 3.ToString();
                    fields[AOE_I] = 1.ToString();
                    fields[CRIT_CHANCE_I] = 0.12.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 4.ToString();
                    break;

                // CLUBS
                case 31: // femur
                    fields[MIN_DAMAGE_I] = 10.ToString();
                    fields[MAX_DAMAGE_I] = 40.ToString();
                    fields[KNOCKBACK_I] = 1.6.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-16).ToString();
                    fields[DEFENSE_I] = 6.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 24: // wood club
                    fields[MIN_DAMAGE_I] = 3.ToString();
                    fields[MAX_DAMAGE_I] = 12.ToString();
                    fields[KNOCKBACK_I] = 1.4.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-14).ToString();
                    fields[DEFENSE_I] = 4.ToString();
                    fields[BASE_DROP_LEVEL_I] = 3.ToString();
                    fields[CRIT_POWER_I] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 27: // wood mallet
                    fields[MIN_DAMAGE_I] = 5.ToString();
                    fields[MAX_DAMAGE_I] = 26.ToString();
                    fields[SPEED_I] = (-10).ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 3.ToString();
                    fields[BASE_DROP_LEVEL_I] = 25.ToString();
                    fields[MIN_DROP_LEVEL_I] = 10.ToString();
                    fields[CRIT_POWER_I] = 1.8.ToString(CultureInfo.InvariantCulture);
                    break;
                case 26: // lead rod
                    fields[MIN_DAMAGE_I] = 20.ToString();
                    fields[MAX_DAMAGE_I] = 60.ToString();
                    fields[KNOCKBACK_I] = 2.2.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-28).ToString();
                    fields[DEFENSE_I] = 14.ToString();
                    fields[BASE_DROP_LEVEL_I] = 65.ToString();
                    fields[MIN_DROP_LEVEL_I] = 40.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 46: // kudgel
                    fields[MIN_DAMAGE_I] = 30.ToString();
                    fields[MAX_DAMAGE_I] = 70.ToString();
                    fields[KNOCKBACK_I] = 1.8.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-24).ToString();
                    fields[DEFENSE_I] = 8.ToString();
                    fields[BASE_DROP_LEVEL_I] = 80.ToString();
                    fields[MIN_DROP_LEVEL_I] = 60.ToString();
                    fields[CRIT_POWER_I] = 2.2.ToString(CultureInfo.InvariantCulture);
                    break;
                case 28: // the slammer
                    fields[MIN_DAMAGE_I] = 40.ToString();
                    fields[MAX_DAMAGE_I] = 105.ToString();
                    fields[KNOCKBACK_I] = 2.4.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-32).ToString();
                    fields[DEFENSE_I] = 12.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 29: // galaxy hammer
                    fields[MIN_DAMAGE_I] = 60.ToString();
                    fields[MAX_DAMAGE_I] = 120.ToString();
                    fields[KNOCKBACK_I] = 2.ToString();
                    fields[SPEED_I] = (-14).ToString();
                    fields[DEFENSE_I] = 10.ToString();
                    fields[AOE_I] = 3.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 55: // dwarf hammer
                    fields[MIN_DAMAGE_I] = 140.ToString();
                    fields[MAX_DAMAGE_I] = 180.ToString();
                    fields[KNOCKBACK_I] = 2.2.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-24).ToString();
                    fields[DEFENSE_I] = 20.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 58: // dragontooth club
                    fields[MIN_DAMAGE_I] = 100.ToString();
                    fields[MAX_DAMAGE_I] = 215.ToString();
                    fields[KNOCKBACK_I] = 2.ToString();
                    fields[SPEED_I] = (-16).ToString();
                    fields[DEFENSE_I] = 12.ToString();
                    fields[CRIT_POWER_I] = 3.ToString();
                    break;
                case 63: // infinity gavel
                    fields[MIN_DAMAGE_I] = 120.ToString();
                    fields[MAX_DAMAGE_I] = 200.ToString();
                    fields[KNOCKBACK_I] = 2.ToString();
                    fields[SPEED_I] = (-12).ToString();
                    fields[DEFENSE_I] = 15.ToString();
                    fields[AOE_I] = 3.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;

                // DEFENSE SWORDS
                case 0: // rusty sword
                    fields[MIN_DAMAGE_I] = 3.ToString();
                    fields[MAX_DAMAGE_I] = 7.ToString();
                    fields[SPEED_I] = (-1).ToString();
                    fields[DEFENSE_I] = 1.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 1: // silver saber
                    fields[MIN_DAMAGE_I] = 4.ToString();
                    fields[MAX_DAMAGE_I] = 8.ToString();
                    fields[SPEED_I] = (-1).ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[DEFENSE_I] = 2.ToString();
                    fields[TYPE_I] = 3.ToString();
                    fields[BASE_DROP_LEVEL_I] = 25.ToString();
                    fields[MIN_DROP_LEVEL_I] = 10.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 6: // iron edge
                    fields[MIN_DAMAGE_I] = 12.ToString();
                    fields[MAX_DAMAGE_I] = 25.ToString();
                    fields[DEFENSE_I] = 2.ToString();
                    fields[BASE_DROP_LEVEL_I] = 40.ToString();
                    fields[MIN_DROP_LEVEL_I] = 15.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 3: // holy blade
                    fields[MIN_DAMAGE_I] = 45.ToString();
                    fields[MAX_DAMAGE_I] = 60.ToString();
                    fields[SPEED_I] = (-2).ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[DEFENSE_I] = 4.ToString();
                    fields[AOE_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 14: // neptune's glaive
                    fields[MIN_DAMAGE_I] = 42.ToString();
                    fields[MAX_DAMAGE_I] = 68.ToString();
                    fields[KNOCKBACK_I] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-2).ToString();
                    fields[PRECISION_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 10: // claymore
                    fields[MIN_DAMAGE_I] = 24.ToString();
                    fields[MAX_DAMAGE_I] = 48.ToString();
                    fields[KNOCKBACK_I] = 1.3.ToString(CultureInfo.InvariantCulture);
                    fields[DEFENSE_I] = 8.ToString();
                    fields[BASE_DROP_LEVEL_I] = 75.ToString();
                    fields[MIN_DROP_LEVEL_I] = 50.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 7: // templar's blade
                    fields[MIN_DAMAGE_I] = 44.ToString();
                    fields[MAX_DAMAGE_I] = 62.ToString();
                    fields[SPEED_I] = (-4).ToString();
                    fields[DEFENSE_I] = 4.ToString();
                    fields[BASE_DROP_LEVEL_I] = 100.ToString();
                    fields[MIN_DROP_LEVEL_I] = 70.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 48: // yeti's tooth
                    fields[MIN_DAMAGE_I] = 26.ToString();
                    fields[MAX_DAMAGE_I] = 42.ToString();
                    fields[KNOCKBACK_I] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[TYPE_I] = 3.ToString();
                    fields[BASE_DROP_LEVEL_I] = 60.ToString();
                    fields[MIN_DROP_LEVEL_I] = 40.ToString();
                    fields[AOE_I] = 2.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.6.ToString(CultureInfo.InvariantCulture);
                    break;
                case 60: // ossified blade
                    fields[MIN_DAMAGE_I] = 48.ToString();
                    fields[MAX_DAMAGE_I] = 68.ToString();
                    fields[DEFENSE_I] = 3.ToString();
                    fields[TYPE_I] = 3.ToString();
                    fields[BASE_DROP_LEVEL_I] = (-1).ToString();
                    fields[MIN_DROP_LEVEL_I] = (-1).ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 52: // tempered broadsword
                    fields[MIN_DAMAGE_I] = 60.ToString();
                    fields[MAX_DAMAGE_I] = 80.ToString();
                    fields[DEFENSE_I] = 6.ToString();
                    fields[BASE_DROP_LEVEL_I] = 120.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.55.ToString(CultureInfo.InvariantCulture);
                    break;
                case 54: // dwarf sword
                    fields[MIN_DAMAGE_I] = 120.ToString();
                    fields[MAX_DAMAGE_I] = 140.ToString();
                    fields[SPEED_I] = (-4).ToString();
                    fields[DEFENSE_I] = 8.ToString();
                    fields[TYPE_I] = 3.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;

                // BACHELOR(ETTE) WEAPONS
                case 40: // abby
                    fields[KNOCKBACK_I] = 0.4.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 4.ToString();
                    fields[PRECISION_I] = 4.ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 42: // haley
                    fields[SPEED_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 39: // leah
                    fields[SPEED_I] = 0.ToString();
                    fields[CRIT_CHANCE_I] = 0.08.ToString(CultureInfo.InvariantCulture);
                    break;
                case 36: // maru
                    fields[SPEED_I] = 0.ToString();
                    fields[DEFENSE_I] = 1.ToString();
                    break;
                case 38: // penny
                    fields[SPEED_I] = 0.ToString();
                    fields[TYPE_I] = 3.ToString();
                    fields[CRIT_CHANCE_I] = 0.04.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 25: // alex
                    fields[KNOCKBACK_I] = 1.4.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-4).ToString();
                    break;
                case 35: // eliott
                    fields[KNOCKBACK_I] = 0.3.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 0.ToString();
                    fields[PRECISION_I] = 2.ToString();
                    fields[DEFENSE_I] = (-2).ToString();
                    fields[CRIT_CHANCE_I] = 0.25.ToString(CultureInfo.InvariantCulture);
                    fields[CRIT_POWER_I] = 2.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 37: // harvey
                    fields[KNOCKBACK_I] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = 0.ToString();
                    fields[DEFENSE_I] = 1.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 30: // sam
                    fields[SPEED_I] = (-12).ToString();
                    fields[DEFENSE_I] = 2.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;
                case 41: // seb
                    fields[KNOCKBACK_I] = 1.2.ToString(CultureInfo.InvariantCulture);
                    fields[SPEED_I] = (-8).ToString();
                    fields[DEFENSE_I] = 4.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;

                // SLINGSHOTS
                case 32: // regular
                    fields[CRIT_CHANCE_I] = 0.ToString();
                    fields[CRIT_POWER_I] = 1.ToString();
                    break;
                case 33: // master
                    fields[CRIT_CHANCE_I] = 0.ToString();
                    fields[CRIT_POWER_I] = 1.ToString();
                    break;
                case 34: // galaxy
                    fields[CRIT_CHANCE_I] = 0.ToString();
                    fields[CRIT_POWER_I] = 1.ToString();
                    break;

                // SCYTHES
                case 47: // regular
                    fields[AOE_I] = 6.ToString();
                    fields[CRIT_POWER_I] = 1.5.ToString(CultureInfo.InvariantCulture);
                    break;
                case 53: // golden
                    fields[MIN_DAMAGE_I] = 10.ToString();
                    fields[AOE_I] = 12.ToString();
                    fields[CRIT_POWER_I] = 2.ToString();
                    break;

                    #endregion weapon switch-case
            }

            data[key] = string.Join('/', fields);
        }
    }

    /// <summary>Edits location string data with custom legendary sword rhyme.</summary>
    private static void EditLocationsStrings(IAssetData asset)
    {
        if (!ModEntry.Config.TrulyLegendaryGalaxySword) return;

        var data = asset.AsDictionary<string, string>().Data;
        data["Town_DwarfGrave_Translated"] = ModEntry.i18n.Get("locations.Town_DwarfGrave_Translated");
    }

    /// <summary>Edits strings data with custom legendary sword reward prompt.</summary>
    private static void EditStringsFromCSFilesStrings(IAssetData asset)
    {
        if (!ModEntry.Config.TrulyLegendaryGalaxySword) return;

        var data = asset.AsDictionary<string, string>().Data;
        data["MeleeWeapon.cs.14122"] = ModEntry.i18n.Get("csfiles.MeleeWeapon.cs.14122");
    }

    #endregion editor callbacks
}