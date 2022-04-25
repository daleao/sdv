namespace DaLion.Stardew.Tweaks.Framework.Events;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using Common.Extensions.Collections;
using Common.Extensions.Reflection;

#endregion using directives

/// <summary>Interface for an event that can be hooked or unhooked.</summary>
internal interface IEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public void Hook();

    /// <summary>Unhook this event from the event listener.</summary>
    public void Unhook();

    /// <summary>Hook all <see cref="IEvent"/> types in the current assembly.</summary>
    internal static void HookAll()
    {
        AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IEvent)))
            .Where(t => t.IsAssignableTo(typeof(IEvent)) && !t.IsAbstract)
            .Select(t => (IEvent)t.RequireConstructor().Invoke(Array.Empty<object>()))
            .ForEach(e => e.Hook());
    }
}