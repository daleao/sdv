namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Extensions for the <see cref="Assembly"/> class.</summary>
public static class AssemblyExtensions
{
    /// <summary>Checks whether the <paramref name="assembly"/> was built in Debug mode.</summary>
    /// <param name="assembly">The <see cref="Assembly"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="assembly"/> contains the 'IsJITTrackingEnabled' attribute, otherwise <see langword="false"/>.</returns>
    public static bool IsDebugBuild(this Assembly assembly)
    {
        return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled);
    }
}
