namespace DaLion.Shared.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using HarmonyLib;

#endregion using directives

/// <summary>Handles mod-provided <see cref="IConsoleCommand"/>s.</summary>
internal sealed class CommandHandler
{
    /// <inheritdoc cref="ICommandHelper"/>
    private readonly ICommandHelper _commandHelper;

    /// <summary>Cache of handled <see cref="IConsoleCommand"/> instances.</summary>
    private readonly Dictionary<string, IConsoleCommand> _handledCommands = new();

    /// <summary>Initializes a new instance of the <see cref="CommandHandler"/> class.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    internal CommandHandler(ICommandHelper helper)
    {
        this._commandHelper = helper;
    }

    /// <summary>Gets the <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</summary>
    public string EntryCommand { get; private set; } = null!; // set in register

    /// <summary>Gets the human-readable name of the providing mod.</summary>
    public string Mod { get; private set; } = null!; // set in register

    /// <summary>Implicitly registers <see cref="IConsoleCommand"/> types in the specified namespace.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="namespace">The desired namespace.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    internal static void FromNamespace(ICommandHelper helper, string @namespace, string entry, string mod)
    {
        Log.D($"[CommandHandler]: Gathering commands in {@namespace}...");
        new CommandHandler(helper).HandleImplicitly(helper, t => t.Namespace?.StartsWith(@namespace) == true).Register(entry, mod);
    }

    /// <summary>Implicitly registers <see cref="IConsoleCommand"/> types with the specified attribute.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    internal static void WithAttribute<TAttribute>(ICommandHelper helper)
        where TAttribute : Attribute
    {
        Log.D($"[CommandHandler]: Gathering commands with {nameof(TAttribute)}...");
        new CommandHandler(helper).HandleImplicitly(helper, t => t.GetCustomAttribute<TAttribute>() is not null);
    }

    /// <summary>Registers the entry command and name for this module.</summary>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    internal void Register(string entry, string mod)
    {
        this.EntryCommand = entry;
        this.Mod = mod;
        var documentation =
            $"The entry point for all {mod} console commands. Type `{entry} help` to list available commands.";
        this._commandHelper.Add(entry, documentation, this.Entry);
    }

    /// <summary>Handles the entry command for this module, delegating to the appropriate <see cref="IConsoleCommand"/>.</summary>
    /// <param name="command">The entry command.</param>
    /// <param name="args">The supplied arguments.</param>
    internal void Entry(string command, string[] args)
    {
        if (args.Length == 0)
        {
            Log.I(
                $"This is the entry point for all {this.Mod} console commands. Use it by specifying a command to be executed. " +
                $"For example, typing `{command} help` will invoke the `help` command, which lists all available commands.");
            return;
        }

        if (string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
        {
            var result = "Available commands:";
            this._handledCommands.Values.Distinct().ForEach(c =>
            {
                result +=
                    $"\n\t-{command} {c.Triggers.First()}";
            });
            Log.I(result);
            return;
        }

        if (!this._handledCommands.TryGetValue(args[0].ToLowerInvariant(), out var handled))
        {
            Log.W($"{args[0]} is not a valid command. Use `{command} help` to see available sub-commands.");
            return;
        }

        if (args.Length > 1 && (string.Equals(args[1], "help", StringComparison.InvariantCultureIgnoreCase) ||
                                string.Equals(args[1], "doc", StringComparison.InvariantCultureIgnoreCase)))
        {
            Log.I(
                $"{handled.Documentation}\n\nAliases: {string.Join(',', handled.Triggers.Skip(1).Select(t => "`" + t + "`"))}");
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save before running this command.");
            return;
        }

        handled.Callback(args.Skip(1).ToArray());
    }

    /// <summary>Implicitly handles <see cref="IConsoleCommand"/> types using reflection.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="predicate">An optional condition with which to limit the scope of handled <see cref="IConsoleCommand"/>s.</param>
    private CommandHandler HandleImplicitly(ICommandHelper helper, Func<Type, bool>? predicate = null)
    {
        predicate ??= t => true;
        var commandTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IConsoleCommand)))
            .Where(t => t.IsAssignableTo(typeof(IConsoleCommand)) && !t.IsAbstract && predicate(t))
            .ToArray();

        Log.D($"[CommandHandler]: Found {commandTypes.Length} command classes. Instantiating commands...");
        foreach (var c in commandTypes)
        {
            try
            {
#if RELEASE
                var debugAttribute = c.GetCustomAttribute<DebugAttribute>();
                if (debugAttribute is not null)
                {
                    continue;
                }
#endif

                var deprecatedAttr = c.GetCustomAttribute<DeprecatedAttribute>();
                if (deprecatedAttr is not null)
                {
                    continue;
                }

                var command = (IConsoleCommand)c
                    .GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] { this.GetType() },
                        null)!
                    .Invoke(new object?[] { this });
                foreach (var trigger in command.Triggers)
                {
                    this._handledCommands.Add(trigger, command);
                }

                Log.D($"[CommandHandler]: Handling {command.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.E($"[CommandHandler]: Failed to handle {c.Name}.\n{ex}");
            }
        }

        Log.D("[CommandHandler]: Command initialization completed.");
        return this;
    }
}
