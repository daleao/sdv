namespace DaLion.Stardew.Professions.Framework.Utility;

internal static class Experience
{
    internal static int[] RequiredPerVanillaLevel { get; } =
        {100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000};

    internal static int Ceiling =>
        15000 + (ModEntry.Config.EnablePrestige ? (int) ModEntry.Config.RequiredExpPerExtendedLevel * 10 : 0);
}