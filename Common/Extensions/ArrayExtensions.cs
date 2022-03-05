namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System.Linq;

#endregion using directives

public static class ArrayExtensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return array.Skip(offset).Take(length).ToArray();
    }
}