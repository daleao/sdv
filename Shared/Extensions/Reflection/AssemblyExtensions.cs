namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

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

    /// <summary>Calculate MD5 checksum for the <paramref name="assembly"/>.</summary>
    /// <param name="assembly">The <see cref="Assembly"/>.</param>
    /// <returns>The <paramref name="assembly"/>'s MD5 checksum as <see cref="string"/>.</returns>
    public static string CalculateMd5(this Assembly assembly)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(assembly.Location);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}
