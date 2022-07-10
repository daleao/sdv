namespace DaLion.Common.Commands;

#region using directives

using Extensions.Collections;

/* Unmerged change from project 'ImmersiveRings'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/

/* Unmerged change from project 'ImmersiveTools'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/

/* Unmerged change from project 'ImmersiveTweaks'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/

/* Unmerged change from project 'ImmersivePonds'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/

/* Unmerged change from project 'ImmersiveAlchemy'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/

/* Unmerged change from project 'ImmersiveArsenal'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/

/* Unmerged change from project 'ImmersiveProfessions'
Before:
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;
After:
using Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections;
*/
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Handles mod-provided console commands.</summary>
internal class CommandHandler
{
    /// <summary>Cache of handled <see cref="IConsoleCommand"/> instances.</summary>
    private readonly Dictionary<string, IConsoleCommand> _HandledCommands = new();

    /// <inheritdoc cref="ICommandHelper"/>
    private readonly ICommandHelper _CommandHelper;

    /// <summary>The <see cref="string"/> used as entry for all handled commands.</summary>
    public string EntryCommand = null!;

    /// <summary>Human-readable name of the mod providing commands.</summary>
    public string Mod = null!;

    /// <summary>Construct an instance.</summary>
    /// <param name="helper">Provides an API for managing console commands.</param>
    internal CommandHandler(ICommandHelper helper)
    {
        _CommandHelper = helper;

        Log.D("[CommandHandler]: Gathering commands...");
        var commandTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IConsoleCommand)))
            .Where(t => t.IsAssignableTo(typeof(IConsoleCommand)) && !t.IsAbstract)
            .ToArray();

        Log.D($"[CommandHandler]: Found {commandTypes.Length} command classes. Initializing commands...");
        foreach (var c in commandTypes)
        {
            try
            {
                var command = (IConsoleCommand)c
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { GetType() }, null)!
                    .Invoke(new object?[] { this });
                _HandledCommands.Add(command.Trigger, command);
                Log.D($"[CommandHandler]: Handling {command.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.E($"[CommandHandler]: Failed to handle {c.Name}.\n{ex}");
            }
        }

        Log.D("[CommandHandler] Command initialization completed.");
    }

    /// <summary>Register the entry command and name for this module.</summary>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled commands.</param>
    /// <param name="mod">Human-readable name of the mod providing commands.</param>
    internal void Register(string entry, string mod)
    {
        EntryCommand = entry;
        Mod = mod;
        var documentation =
            $"The entry point for all {mod} console commands. Type `{entry} help` to list available commands.";
        _CommandHelper.Add(entry, documentation, Entry);
    }

    /// <summary>Handles the entry command for this module, delegating to the appropriate <see cref="IConsoleCommand"/>.</summary>
    /// <param name="command">The entry command.</param>
    /// <param name="args">The supplied arguments.</param>
    internal void Entry(string command, string[] args)
    {
        if (args.Length <= 0)
        {
            Log.I(
                $"This is the entry point for all {Mod} console commands. Use it by specifying a command to be executed. " +
                $"For example, typing `{command} help` will invoke the `help` command, which lists all available commands.");
            return;
        }

        if (string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
        {
            var result = "Available commands:";
            _HandledCommands.Values.ForEach(c => { result += $"\n\t-{command} {c.Trigger}"; });
            Log.I(result);
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save before running this command.");
            return;
        }

        if (!_HandledCommands.TryGetValue(args[0].ToLowerInvariant(), out var handled))
        {
            Log.W($"{args[0]} is not a valid command. Use `{command} help` to see available sub-commands.");
            return;
        }

        handled.Callback(args.Skip(1).ToArray());
    }
}