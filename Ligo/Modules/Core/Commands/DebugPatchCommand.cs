namespace DaLion.Ligo.Modules.Core.Commands;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Core.Patchers;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugPatchCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DebugPatchCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DebugPatchCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "debug_patch", "patch_debug" };

    /// <inheritdoc />
    public override string Documentation => "Applies the DebugPatcher to the specified method. Only works for non-overloaded methods.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length < 1)
        {
            Log.W("Missing arguments. Please specify the target class and method.");
            return;
        }

        try
        {
            var type = args[0].ToType();
            MethodBase target = args.Length > 1 ? type.RequireMethod(args[1]) : type.RequireConstructor();
            new Harmony("DaLion.Debug").Patch(
                target,
                new HarmonyMethod(typeof(DebugPatcher), nameof(DebugPatcher.DebugPrefix)),
                new HarmonyMethod(typeof(DebugPatcher), nameof(DebugPatcher.DebugPostfix)));
            Log.D($"Applied {nameof(DebugPatcher)} to {target.GetFullName()}.");
        }
        catch (MissingTypeException)
        {
            Log.E($"The type {args[0]} was not found.");
        }
        catch (MissingMethodException)
        {
            Log.E($"The method {args[1]} was not found within the type {args[0]}.");
        }
        catch
        {
            Log.E("Something went wrong. The patcher was not applied.");
        }
    }
}
