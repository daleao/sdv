namespace DaLion.Common.Commands;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Extensions.Collections;

#endregion using directives

/// <summary>Handles mod-provided console commands.</summary>
internal class CommandHandler
{
    /// <summary>Cache of handled <see cref="ICommand"/> instances.</summary>
    private readonly Dictionary<string, ICommand> _HandledCommands = new();

    /// <inheritdoc cref="ICommandHelper"/>
    private readonly ICommandHelper _CommandHelper;

    /// <summary>Construct an instance.</summary>
    /// <param name="helper">Provides an API for managing console commands.</param>
    public CommandHandler(ICommandHelper helper)
    {
        _CommandHelper = helper;

        Log.D("[CommandHandler]: Gathering commands...");
        var commandTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(ICommand)))
            .Where(t => t.IsAssignableTo(typeof(ICommand)) && !t.IsAbstract).ToList();

        Log.D($"[CommandHandler]: Found {commandTypes.Count} command classes. Initializing commands...");
        foreach (var c in commandTypes.Select(t =>
                     (ICommand) t.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>()!)))
            _HandledCommands.Add(c.Trigger, c);

        Log.D("[CommandHandler] Command initialization completed.");
    }

    /// <summary>Register the entry command for this module.</summary>
    /// <param name="entry">The command to be used as entry.</param>
    internal void Register(string entry, string mod)
    {
        var documentation =
            $"The entry point for all {mod} console commands. Type `{entry} help` to list available commands.";
        _CommandHelper.Add(entry, documentation, Entry);
    }

    /// <summary>Handles the entry command for this module, delegating to the appropriate <see cref="ICommand"/>.</summary>
    /// <param name="command">The entry command.</param>
    /// <param name="args">The supplied arguments.</param>
    internal void Entry(string command, string[] args)
    {
        if (!args.Any() || string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
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