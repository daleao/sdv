namespace DaLion.Stardew.Alchemy.Framework;

public interface IMixture
{
    int[] FormulaCoefficients { get; }

    int Product { get; }
}