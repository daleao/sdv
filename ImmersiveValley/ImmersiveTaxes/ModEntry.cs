namespace DaLion.Stardew.Taxes;

#region using directives

using Common;
using Common.Data;
using Common.Events;
using Common.Harmony;
using Common.Integrations;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using static System.FormattableString;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static PerScreen<int> LatestAmountDue { get; } = new(() => 0);

    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static IImmersiveProfessionsAPI? ProfessionsAPI { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // initialize data
        ModDataIO.Init(helper.Multiplayer, ModManifest.UniqueID);

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // hook events
        new EventManager(helper.Events).HookAll();

        // apply patches
        new Harmonizer(ModManifest.UniqueID).ApplyAll();

        // register commands
        helper.ConsoleCommands.Add(
            "do_taxes",
            "Check accounting stats for the current season-to-date, or the closing season if checking on the 1st day of the season.",
            DoTaxes
        );
    }

    /// <summary>Calculate projected income tax for the player.</summary>
    private static void DoTaxes(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save before running this command.");
            return;
        }

        var forClosingSeason = Game1.dayOfMonth == 1;
        var income = ModDataIO.ReadFrom<int>(Game1.player, "SeasonIncome");
        var deductible = ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.mariner)
            ? forClosingSeason
                ? ModDataIO.ReadFrom<float>(Game1.player, "DeductionPct")
                : ProfessionsAPI.GetConservationistProjectedTaxBonus(Game1.player)
            : 0f;
        var taxable = (int)(income * (1f - deductible));
        var bracket = Framework.Utils.GetTaxBracket(taxable);
        var due = (int)Math.Round(taxable * bracket);
        Log.I(
            "Accounting " + (forClosingSeason ? "report" : "projections") + " for the " + (forClosingSeason ? "closing" : "current") + " season:" +
            $"\n\t- Income (season-to-date): {income}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Current tax bracket: {bracket:p0}") +
            $"\n\t- Due income tax: {due}g." +
            $"\n\t- Total projected income tax: {due * 28 / Game1.dayOfMonth}g." +
            $"\nRequested on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}."
        );
    }
}