namespace DaLion.Common.Extensions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Xml;

#endregion using directives

/// <summary>Extensions for reading data from <see cref="XmlElement"/> instance.</summary>
public static class XmlElementExtensions
{
    /// <summary>Attempts to read the specified node in the <paramref name="xml"/> element as <see cref="string"/>.</summary>
    /// <param name="xml">The <see cref="XmlElement"/>.</param>
    /// <param name="name">The name of the node.</param>
    /// <param name="innerText">The inner text of the node if it exists, otherwise <see cref="string.Empty"/>.</param>
    /// <returns><see langword="true"/> if the node exists and is non-empty, otherwise <see langword="false"/>.</returns>
    public static bool TryRead(this XmlElement xml, string name, out string innerText)
    {
        innerText = string.Empty;
        if (xml.SelectSingleNode(name) is not XmlElement node)
        {
            return false;
        }

        innerText = node.InnerText;
        return string.IsNullOrEmpty(innerText);
    }

    /// <summary>Reads the specified node in the <paramref name="xml"/> element as <see cref="string"/>.</summary>
    /// <param name="xml">The <see cref="XmlElement"/>.</param>
    /// <param name="name">The name of the node.</param>
    /// <returns>The inner text of the node if it exists, otherwise <see cref="string.Empty"/>.</returns>
    public static string Read(this XmlElement xml, string name)
    {
        if (!xml.TryRead(name, out var result))
        {
            ThrowHelper.ThrowArgumentException($"Element [{name}] was not found in {xml.Name}.");
        }

        return result;
    }

    /// <summary>Attempts to read the specified node in the <paramref name="xml"/> element.</summary>
    /// <typeparam name="T">The expected type with which to parse the node's inner text.</typeparam>
    /// <param name="xml">The <see cref="XmlElement"/>.</param>
    /// <param name="name">The name of the node.</param>
    /// <param name="result">The parsed value if successful, otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the node exists and can be parsed, otherwise <see langword="false"/>.</returns>
    public static bool TryReadAs<T>(this XmlElement xml, string name, [NotNullWhen(true)] out T? result)
    {
        result = default(T);
        if (xml.SelectSingleNode(name) is not XmlElement node)
        {
            return false;
        }

        var innerText = node.InnerText;
        return !string.IsNullOrEmpty(innerText) && innerText.TryParse(out result);
    }

    /// <summary>Reads the specified node in the <paramref name="xml"/> element and parses it to a generic type.</summary>
    /// <typeparam name="T">The expected type with which to parse the node's inner text.</typeparam>
    /// <param name="xml">The <see cref="XmlElement"/>.</param>
    /// <param name="name">The name of the node.</param>
    /// <returns>The parsed value if successful, otherwise <see langword="null"/>.</returns>
    public static T Read<T>(this XmlElement xml, string name)
    {
        if (!xml.TryReadAs<T>(name, out var result))
        {
            ThrowHelper.ThrowArgumentException($"Element [{name}] was not found in {xml.Name}.");
        }

        return result;
    }
}
