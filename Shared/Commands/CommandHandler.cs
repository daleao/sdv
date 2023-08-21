namespace DaLion.Shared.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using HarmonyLib;
using StardewModdingAPI;

#endregion using directives

/// <summary>Handles mod-provided <see cref="IConsoleCommand"/>s.</summary>
internal sealed class CommandHandler
{
    /// <inheritdoc cref="ICommandHelper"/>
    private readonly ICommandHelper _commandHelper;

    /// <summary>Cache of handled <see cref="IConsoleCommand"/> instances.</summary>
    private readonly Dictionary<string, IConsoleCommand> _handledCommands = new();

    /// <summary>An optional conditional expression that prevents the entry command from being executed.</summary>
    private Func<bool>? _conditional;

    /// <summary>Initializes a new instance of the <see cref="CommandHandler"/> class.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    internal CommandHandler(ICommandHelper helper)
    {
        this._commandHelper = helper;
    }

    /// <summary>Gets the <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</summary>
    internal string EntryCommand { get; private set; } = null!; // set in register

    /// <summary>Gets the human-readable name of the providing mod.</summary>
    internal string Mod { get; private set; } = null!; // set in register

    /// <summary>Implicitly registers all <see cref="IConsoleCommand"/> types in the assembly using reflection.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    internal static CommandHandler HandeAll(ICommandHelper helper, string mod, string entry, Func<bool>? conditional = null)
    {
        Log.D($"[CommandHandler]: Gathering all commands...");
        return new CommandHandler(helper)
            .HandleImplicitly()
            .Register(mod, entry, conditional);
    }

    /// <summary>Implicitly registers <see cref="IConsoleCommand"/> types in the specified namespace.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="namespace">The desired namespace.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    internal static CommandHandler HandleFromNamespace(ICommandHelper helper, string @namespace, string mod, string entry, Func<bool>? conditional = null)
    {
        Log.D($"[CommandHandler]: Gathering commands in {@namespace}...");
        return new CommandHandler(helper)
            .HandleImplicitly(t => t.Namespace?.Contains(@namespace) == true)
            .Register(mod, entry, conditional);
    }

    /// <summary>Implicitly registers <see cref="IConsoleCommand"/> types with the specified attribute.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    internal static CommandHandler HandleWithAttribute<TAttribute>(ICommandHelper helper, string mod, string entry, Func<bool>? conditional = null)
        where TAttribute : Attribute
    {
        Log.D($"[CommandHandler]: Gathering commands with {nameof(TAttribute)}...");
        return new CommandHandler(helper)
            .HandleImplicitly(t => t.GetCustomAttribute<TAttribute>() is not null)
            .Register(mod, entry, conditional);
    }

    /// <summary>Registers the entry command and name for this module.</summary>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    internal CommandHandler Register(string mod, string entry, Func<bool>? conditional = null)
    {
        if (this._handledCommands.Count == 0)
        {
            Log.D($"[CommandHandler]: The mod {mod} did not provide any console commands.");
            return this;
        }

        this.Mod = mod;
        this.EntryCommand = entry;
        this._conditional = conditional;
        var documentation =
            $"The entry point for all {mod} console commands. Type `{entry} help` to list available commands.";
        this._commandHelper.Add(entry, documentation, this.Entry);
        return this;
    }

    /// <summary>Handles the entry command for this module, delegating to the appropriate <see cref="IConsoleCommand"/>.</summary>
    /// <param name="command">The entry command.</param>
    /// <param name="args">The supplied arguments.</param>
    internal void Entry(string command, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.I(
                $"This is the entry point for all {this.Mod} console commands. Use it by specifying a command to be executed. " +
                $"For example, typing `{command} help` will invoke the `help` command, which lists all available commands.");
            return;
        }

        if (string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
        {
            var result = new StringBuilder("Available commands:");
            this._handledCommands.Values.Distinct().ForEach(c =>
            {
                result.Append($"\n\t-{command} {c.Triggers[0]}");
            });

            Log.I(result.ToString());
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

        if (!this._conditional?.Invoke() ?? false)
        {
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save before running a command.");
            return;
        }

        handled.Callback(args[0], args.Skip(1).ToArray());
    }

    /// <summary>Implicitly handles <see cref="IConsoleCommand"/> types using reflection.</summary>
    /// <param name="predicate">An optional condition with which to limit the scope of handled <see cref="IConsoleCommand"/>s.</param>
    private CommandHandler HandleImplicitly(Func<Type, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var commandTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IConsoleCommand)))
            .Where(t => t.IsAssignableTo(typeof(IConsoleCommand)) && !t.IsAbstract && predicate(t))
            .ToArray();

        Log.D($"[CommandHandler]: Found {commandTypes.Length} command classes.");
        if (commandTypes.Length == 0)
        {
            return this;
        }

        Log.D($"[CommandHandler]: Instantiating commands...");
        for (var i = 0; i < commandTypes.Length; i++)
        {
            var commandType = commandTypes[i];
            try
            {
#if RELEASE
                var debugAttribute = commandType.GetCustomAttribute<DebugAttribute>();
                if (debugAttribute is not null)
                {
                    continue;
                }
#endif

                var ignoreAttribute = commandType.GetCustomAttribute<ImplicitIgnoreAttribute>();
                if (ignoreAttribute is not null)
                {
                    Log.D($"[CommandHandler]: {commandType.Name} is marked to be ignored.");
                    continue;
                }

                var command = (IConsoleCommand)commandType
                    .GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] { this.GetType() },
                        null)!
                    .Invoke(new object?[] { this });
                for (var j = 0; j < command.Triggers.Length; j++)
                {
                    this._handledCommands.Add(command.Triggers[j], command);
                }

                Log.T($"[CommandHandler]: Handling {command.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.E($"[CommandHandler]: Failed to handle {commandType.Name}.\n{ex}");
            }
        }

        Log.T("[CommandHandler]: Command initialization completed.");
        return this;
    }
}
