namespace DaLion.Stardew.Alchemy.Framework;

internal static class ToxicityManager
{
    public const int BASE_TOLERANCE_I = 100;

    public static int MaxTolerance { get; }

    public static int OverdoseThreshold { get; }

    private static int _ToxicityValueValue;

    public static int ToxicityValue
    {
        get => _ToxicityValueValue;
        set => _ToxicityValueValue = value;
    }
}