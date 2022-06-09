namespace DaLion.Stardew.Professions.Framework.Utility;

internal static class Experience
{
    internal const int VANILLA_CAP_I = 15000;

    internal static readonly int[] VanillaExpPerLevel = { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
    
    internal static int PrestigeCap => VANILLA_CAP_I + (int) ModEntry.Config.RequiredExpPerExtendedLevel * 10;
}