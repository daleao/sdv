namespace DaLion.Shared.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;

#endregion using directives

/// <summary>Handles mod-provided <see cref="IConsoleCommand"/>s.</summary>
public sealed class CommandHandler
{
    /// <inheritdoc cref="ICommandHelper"/>
    private readonly ICommandHelper _commandHelper;

    /// <summary>Cache of handled <see cref="IConsoleCommand"/> instances.</summary>
    private readonly Dictionary<string, IConsoleCommand> _handledCommands = [];

    /// <inheritdoc cref="Logger"/>
    private readonly Logger _log;

    /// <summary>An optional conditional expression that prevents the entry command from being executed.</summary>
    private Func<bool>? _conditional;

    /// <summary>Initializes a new instance of the <see cref="CommandHandler"/> class.</summary>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    private CommandHandler(ICommandHelper helper, Logger logger)
    {
        this._commandHelper = helper;
        this._log = logger;
    }

    /// <summary>Gets the <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</summary>
    public string EntryCommand { get; private set; } = null!; // set in register

    /// <summary>Gets the human-readable name of the providing mod.</summary>
    public string Mod { get; private set; } = null!; // set in register

    /// <summary>Implicitly registers all <see cref="IConsoleCommand"/> types in the specified <paramref name="assembly"/> using reflection.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    public static CommandHandler HandleAll(
        Assembly assembly,
        ICommandHelper helper,
        Logger logger,
        string mod,
        string entry,
        Func<bool>? conditional = null)
    {
        logger.D("[CommandHandler]: Gathering all commands...");
        return new CommandHandler(helper, logger)
            .HandleImplicitly(assembly)
            .Register(mod, entry, conditional);
    }

    /// <summary>Implicitly registers only the <see cref="IConsoleCommand"/> types iin the specified <paramref name="assembly"/> which are also within the specified <paramref name="namespace"/>.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="namespace">The desired namespace.</param>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    public static CommandHandler HandleFromNamespace(
        Assembly assembly,
        string @namespace,
        ICommandHelper helper,
        Logger logger,
        string mod,
        string entry,
        Func<bool>? conditional = null)
    {
        logger.D($"[CommandHandler]: Gathering commands in {@namespace}...");
        return new CommandHandler(helper, logger)
            .HandleImplicitly(assembly, t => t.Namespace?.StartsWith(@namespace) == true)
            .Register(mod, entry, conditional);
    }

    /// <summary>Implicitly registers only the <see cref="IConsoleCommand"/> types in the specified <paramref name="assembly"/> which are also decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="helper">The <see cref="ICommandHelper"/> API for the current mod.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    public static CommandHandler HandleWithAttribute<TAttribute>(
        Assembly assembly,
        ICommandHelper helper,
        Logger logger,
        string mod,
        string entry,
        Func<bool>? conditional = null)
        where TAttribute : Attribute
    {
        logger.D($"[CommandHandler]: Gathering commands with {nameof(TAttribute)}...");
        return new CommandHandler(helper, logger)
            .HandleImplicitly(assembly, t => t.HasAttribute<TAttribute>())
            .Register(mod, entry, conditional);
    }

    /// <summary>Handles the specified <see cref="IConsoleCommand"/> instance.</summary>
    /// <param name="command">A <see cref="IConsoleCommand"/>.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    public CommandHandler Handle(IConsoleCommand command)
    {
        foreach (var trigger in command.Triggers)
        {
            this._handledCommands.TryAdd(trigger, command);
        }

        return this;
    }

    /// <summary>Handles the specified <see cref="IConsoleCommand"/> instance.</summary>
    /// <typeparam name="TCommand">A type which implements <see cref="IConsoleCommand"/>.</typeparam>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    public CommandHandler Handle<TCommand>()
        where TCommand : IConsoleCommand
    {
        var command = (IConsoleCommand)typeof(TCommand)
            .RequireConstructor(this.GetType())
            .Invoke([this]);
        return this.Handle(command);
    }

    /// <summary>Registers the entry command and name for this handler.</summary>
    /// <param name="mod">Human-readable name of the providing mod.</param>
    /// <param name="entry">The <see cref="string"/> used as entry for all handled <see cref="IConsoleCommand"/>s.</param>
    /// <param name="conditional">An optional conditional expression that prevents the entry command from being executed.</param>
    /// <returns>The <see cref="CommandHandler"/> instance.</returns>
    public CommandHandler Register(string mod, string entry, Func<bool>? conditional = null)
    {
        if (this._handledCommands.Count == 0)
        {
            this._log.D($"[CommandHandler]: The mod {mod} did not provide any console commands.");
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
    public void Entry(string command, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            this._log.I(
                $"This is the entry point for all {this.Mod} console commands. Use it by specifying a sub-command to be executed. " +
                $"For example, typing `{command} help` will invoke the `help` command, which lists all available sub-commands.");
            return;
        }

        if (string.Equals(args[0], "help", StringComparison.InvariantCultureIgnoreCase))
        {
            var result = new StringBuilder("Available sub-commands:");
            this._handledCommands.Values.Distinct().ForEach(c =>
            {
                result.Append($"\n\t-{command} {c.Triggers[0]}");
            });

            this._log.I(result.ToString());
            return;
        }

        if (!this._handledCommands.TryGetValue(args[0].ToLowerInvariant(), out var handled))
        {
            this._log.W($"{args[0]} is not a valid command. Use `{command} help` to see available sub-commands.");
            return;
        }

        if (!this._conditional?.Invoke() ?? false)
        {
            return;
        }

        if (!Context.IsWorldReady)
        {
            this._log.W("You must load a save before running a command.");
            return;
        }

        handled.Callback(args[0], args.Skip(1).ToArray());
    }

    /// <summary>Implicitly handles <see cref="IConsoleCommand"/> types using reflection.</summary>
    /// <param name="assembly">The assembly to search within.</param>
    /// <param name="predicate">An optional condition with which to limit the scope of handled <see cref="IConsoleCommand"/>s.</param>
    private CommandHandler HandleImplicitly(Assembly assembly, Func<Type, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var commandTypes = AccessTools
            .GetTypesFromAssembly(assembly)
            .Where(t => t.IsAssignableTo(typeof(IConsoleCommand)) && !t.IsAbstract && predicate(t))
            .ToArray();

        this._log.D($"[CommandHandler]: Found {commandTypes.Length} command classes.");
        if (commandTypes.Length == 0)
        {
            return this;
        }

        this._log.D("[CommandHandler]: Instantiating commands...");
        foreach (var commandType in commandTypes)
        {
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
                    this._log.D($"[CommandHandler]: {commandType.Name} is marked to be ignored.");
                    continue;
                }

                var command = (IConsoleCommand)commandType
                    .RequireConstructor(this.GetType())
                    .Invoke([this]);
                foreach (var trigger in command.Triggers)
                {
                    this._handledCommands.Add(trigger, command);
                }

                this._log.D($"[CommandHandler]: Now handling {command.GetType().Name}");
            }
            catch (Exception ex)
            {
                this._log.E($"[CommandHandler]: Failed to handle {commandType.Name}.\n{ex}");
            }
        }

        this._log.D("[CommandHandler]: Command initialization completed.");
        return this;
    }
}
