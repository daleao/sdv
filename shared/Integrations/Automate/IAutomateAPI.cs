namespace DaLion.Shared.Integrations.Automate;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The API provided by Automate.</summary>
public interface IAutomateApi
{
    /// <summary>Adds an automation factory.</summary>
    /// <param name="factory">An automation factory which construct machines, containers, and connectors.</param>
    void AddFactory(IAutomationFactory factory);

    /// <summary>Gets the status of machines in a tile area. This is a specialized API for Data Layers and similar mods.</summary>
    /// <param name="location">The location for which to display data.</param>
    /// <param name="tileArea">The tile area for which to display data.</param>
    /// <returns>The internal state enum of each machine in the <paramref name="tileArea"/>.</returns>
    IDictionary<Vector2, int> GetMachineStates(GameLocation location, Rectangle tileArea);
}
