namespace DaLion.Shared.Harmony;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Provides an API for abstracting common transpiler operations.</summary>
// ReSharper disable once InconsistentNaming
public sealed class ILHelper
{
    private readonly Stack<int> _indexStack = new();
    private readonly List<CodeInstruction> _instructions;

    /// <summary>Initializes a new instance of the <see cref="ILHelper"/> class.</summary>
    /// <param name="original">A <see cref="MethodBase"/> representation of the original method.</param>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to be modified.</param>
    public ILHelper(MethodBase original, IEnumerable<CodeInstruction> instructions)
    {
        this.Original = original;
        this._instructions = instructions.ToList();
        this.Locals = this._instructions
            .Where(instruction => (instruction.IsLdloc() || instruction.IsStloc()) && instruction.operand is not null)
            .Select(instruction => (LocalBuilder)instruction.operand)
            .ToHashSet()
            .ToDictionary(lb => lb.LocalIndex, lb => lb);
        this._indexStack.Push(0);
    }

    /// <summary>Specifies the starting point and direction for a pattern search.</summary>
    public enum SearchOption
    {
        /// <summary>Find the first occurrence of a pattern by starting from the first instruction and searching in the forward direction.</summary>
        First,

        /// <summary>Find the first occurrence of a pattern by starting from the current instruction and searching in the forward direction.</summary>
        Next,

        /// <summary>Find the first occurrence of a pattern by starting from the current instruction and searching in the reverse direction.</summary>
        Previous,

        /// <summary>Find the first occurrence of a pattern by starting from the last instruction and searching in the reverse direction.</summary>
        Last,
    }

    /// <summary>Gets metadata about the original target method.</summary>
    public MethodBase Original { get; }

    /// <summary>Gets the current list of <see cref="CodeInstruction"/>s that will eventually replace the target method.</summary>
    public IReadOnlyList<CodeInstruction> Instructions => this._instructions;

    /// <summary>Gets a look-up table for easy indexing of <see cref="LocalBuilder"/> objects by their corresponding local index.</summary>
    public IReadOnlyDictionary<int, LocalBuilder> Locals { get; }

    /// <summary>Gets the index currently at the top of the index stack.</summary>
    public int CurrentIndex
    {
        get
        {
            if (this._indexStack.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException(
                    "Tried to access the index stack while it was null or empty.");
            }

            return this._indexStack.Peek();
        }
    }

    /// <summary>Gets the index of the last <see cref="CodeInstruction"/> in the current instruction list.</summary>
    public int LastIndex
    {
        get
        {
            if (this._instructions.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException(
                    "Tried to access the instruction list while it was null or empty.");
            }

            return this._instructions.Count - 1;
        }
    }

    /// <summary>
    ///     Finds the first occurrence of the specified <paramref name="pattern"/> in the active
    ///     <see cref="CodeInstruction"/> list according to the specified <see cref="SearchOption"/>,
    ///     and moves the stack pointer to it.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <param name="search">The <see cref="SearchOption"/>.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Match(CodeInstruction[] pattern, SearchOption search = SearchOption.Next)
    {
        var index = -1;
        switch (search)
        {
            case SearchOption.First or SearchOption.Next:
            {
                var start = search is SearchOption.Next ? this.CurrentIndex + 1 : 0;
                index = this._instructions.IndexOf(pattern, start);
                break;
            }

            case SearchOption.Previous or SearchOption.Last:
            {
                var searchSpace = this._instructions.Clone();
                searchSpace.Reverse();

                var start = search is SearchOption.Previous ? searchSpace.Count - this.CurrentIndex : 0;
                index = searchSpace.Count - searchSpace.IndexOf(pattern.Reverse().ToArray(), start) - pattern.Length;
                break;
            }

            default:
            {
                ThrowHelperExtensions.ThrowUnexpectedEnumValueException(search);
                break;
            }
        }

        if (index < 0)
        {
            ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>
    ///     Finds the first occurrence of the specified <paramref name="pattern"/> in the active
    ///     <see cref="CodeInstruction"/> list according to the specified <see cref="SearchOption"/>,
    ///     without moving the stack pointer to it.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <param name="count">The number of instructions until the first occurrence of <paramref name="pattern"/>.</param>
    /// <param name="search">The <see cref="SearchOption"/>.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Match(CodeInstruction[] pattern, out int count, SearchOption search = SearchOption.Next)
    {
        this.Match(pattern, search);
        var end = this._indexStack.Pop() + 1;
        count = end - this.CurrentIndex;
        return this;
    }

    /// <summary>
    ///     Finds the specified <paramref name="label"/> in the active <see cref="CodeInstruction"/> list and moves the
    ///     stack pointer to it.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> object to match.</param>
    /// <param name="search">The <see cref="SearchOption"/>.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper FindLabel(Label label, SearchOption search = SearchOption.Next)
    {
        var index = -1;
        switch (search)
        {
            case SearchOption.First or SearchOption.Next:
            {
                var start = search is SearchOption.Next ? this.CurrentIndex + 1 : 0;
                index = this._instructions.IndexOf(label, start);
                break;
            }

            case SearchOption.Previous or SearchOption.Last:
            {
                var searchSpace = this._instructions.Clone();
                searchSpace.Reverse();

                var start = search is SearchOption.Previous ? searchSpace.Count - this.CurrentIndex : 0;
                index = searchSpace.Count - searchSpace.IndexOf(label, start) - 1;
                break;
            }

            default:
            {
                ThrowHelperExtensions.ThrowUnexpectedEnumValueException(search);
                break;
            }
        }

        if (index < 0)
        {
            ThrowHelperExtensions.ThrowLabelNotFoundException(label, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>
    ///     Finds the specified <paramref name="label"/> in the active <see cref="CodeInstruction"/> list
    ///     without moving the stack pointer to it.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> object to match.</param>
    /// <param name="count">The number of instructions until the specified of <paramref name="label"/>.</param>
    /// <param name="search">The <see cref="SearchOption"/>.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper FindLabel(Label label, out int count, SearchOption search = SearchOption.Next)
    {
        this.FindLabel(label, search);
        var end = this._indexStack.Pop() + 1;
        count = end - this.CurrentIndex;
        return this;
    }

    /// <summary>Moves the stack pointer an integer number of <paramref name="steps"/>.</summary>
    /// <param name="steps">Number of steps by which to move the stack pointer.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Move(int steps = 1)
    {
        if (this.CurrentIndex + steps < 0 || this.CurrentIndex + steps > this.LastIndex)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("New index is out of range.");
        }

        this._indexStack.Push(this.CurrentIndex + steps);
        return this;
    }

    /// <summary>Moves the stack pointer to the specific <paramref name="index"/>.</summary>
    /// <param name="index">The index to move to.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper GoTo(int index)
    {
        if (index < 0)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("Can't go to a negative index.");
        }

        if (index > this.LastIndex)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("New index is out of range.");
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>Inserts the given <paramref name="instructions"/> at the currently pointed index.</summary>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to insert.</param>
    /// <param name="labels">Some <see cref="Label"/>s to add at the start of the insertion.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    /// <remarks>
    ///     The instruction at the current address is pushed forward, such that the stack pointer continues to point to
    ///     the same instruction after insertion.
    /// </remarks>
    public ILHelper Insert(CodeInstruction[] instructions, Label[]? labels = null)
    {
        if (labels is not null)
        {
            instructions[0].labels.AddRange(labels);
        }

        this._instructions.InsertRange(this.CurrentIndex, instructions);
        this._indexStack.Push(this.CurrentIndex + instructions.Length);
        return this;
    }

    /// <summary>Adds the given <paramref name="instructions"/> to the end of the active <see cref="CodeInstruction"/> list.</summary>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to add.</param>
    /// <param name="labels">Some <see cref="Label"/>s to add at the start of the insertion.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    /// <remarks>The stack pointer is moved to the first instruction in the added sequence.</remarks>
    public ILHelper Add(CodeInstruction[] instructions, Label[]? labels = null)
    {
        if (labels is not null)
        {
            instructions[0].labels.AddRange(labels);
        }

        this._instructions.AddRange(instructions);
        this._indexStack.Push(this.LastIndex - instructions.Length);
        return this;
    }

    /// <summary>
    ///     Gets a copy of the next <paramref name="count"/> <see cref="CodeInstruction"/>s, starting from the currently
    ///     pointed index.
    /// </summary>
    /// <param name="copy">The got code instructions.</param>
    /// <param name="count">Number of code instructions to get.</param>
    /// <param name="removeLabels">Whether to remove the labels of the copied <see cref="CodeInstruction"/>s.</param>
    /// <param name="moveStackPointer">Whether to advance the stack pointer.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Copy(out CodeInstruction[] copy, int count = 1, bool removeLabels = false, bool moveStackPointer = false)
    {
        copy = this._instructions.GetRange(this.CurrentIndex, count).Clone().ToArray();
        if (removeLabels)
        {
            foreach (var instruction in copy)
            {
                instruction.labels.Clear();
            }
        }

        if (moveStackPointer)
        {
            this._indexStack.Push(this._indexStack.Peek() + count);
        }

        return this;
    }

    /// <summary>
    ///     Removes the next <paramref name="count"/> <see cref="CodeInstruction"/>s starting from the currently pointed
    ///     index.
    /// </summary>
    /// <param name="count">Number of code instructions to remove.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Remove(int count = 1)
    {
        if (this.CurrentIndex + count > this.LastIndex)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("Can't remove item out of range.");
        }

        this._instructions.RemoveRange(this.CurrentIndex, count);
        return this;
    }

    /// <summary>Replaces the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="instruction">The <see cref="CodeInstruction"/> to replace with.</param>
    /// <param name="preserveLabels">Whether to preserve the labels at the current <see cref="CodeInstruction"/> before replacement.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper ReplaceWith(CodeInstruction instruction, bool preserveLabels = false)
    {
        if (preserveLabels)
        {
            instruction.labels = this._instructions[this.CurrentIndex].labels;
        }

        this._instructions[this.CurrentIndex] = instruction;
        return this;
    }

    /// <summary>Adds one or more <see cref="Label"/>s to the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="labels">Some of <see cref="Label"/>s to add.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper AddLabels(params Label[] labels)
    {
        this._instructions[this.CurrentIndex].labels.AddRange(labels);
        return this;
    }

    /// <summary>
    ///     Removes the specified <see cref="Label"/>s from the <see cref="CodeInstruction"/> at the currently pointed
    ///     index.
    /// </summary>
    /// <param name="labels">The <see cref="Label"/>s to remove.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper RemoveLabels(params Label[] labels)
    {
        labels.ForEach(l => this.Instructions[this.CurrentIndex].labels.Remove(l));
        return this;
    }

    /// <summary>
    ///     Gets a copy of the <see cref="Label"/>s from the <see cref="CodeInstruction"/> at the currently pointed
    ///     index.
    /// </summary>
    /// <param name="labels">The copied <see cref="Label"/>s.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper GetLabels(out Label[] labels)
    {
        labels = this._instructions[this.CurrentIndex].labels.ToArray();
        return this;
    }

    /// <summary>Replaces the <see cref="Label"/>s of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="labels">The new <see cref="Label"/>s to set.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper SetLabels(params Label[] labels)
    {
        this._instructions[this.CurrentIndex].labels = labels.ToList();
        return this;
    }

    /// <summary>Removes all <see cref="Label"/>s from the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper StripLabels()
    {
        this._instructions[this.CurrentIndex].labels.Clear();
        return this;
    }

    /// <summary>
    ///     Removes all <see cref="Label"/>s from the <see cref="CodeInstruction"/>s at the currently pointed index and
    ///     returns a reference to those <see cref="Label"/>s.
    /// </summary>
    /// <param name="labels">The removed <see cref="Label"/>s.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper StripLabels(out Label[] labels)
    {
        this.GetLabels(out labels);
        return this.StripLabels();
    }

    /// <summary>Returns the <see cref="OpCode"/> of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="opcode">The returned <see cref="OpCode"/>.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper GetOpCode(out OpCode opcode)
    {
        opcode = this._instructions[this.CurrentIndex].opcode;
        return this;
    }

    /// <summary>Changes the <see cref="OpCode"/> of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="opcode">The new <see cref="OpCode"/> to replace with.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper SetOpCode(OpCode opcode)
    {
        this._instructions[this.CurrentIndex].opcode = opcode;
        return this;
    }

    /// <summary>Returns the operand of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="operand">The returned operand.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper GetOperand(out object operand)
    {
        operand = this._instructions[this.CurrentIndex].operand;
        return this;
    }

    /// <summary>Changes the operand of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="operand">The new operand to replace with.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper SetOperand(object operand)
    {
        this._instructions[this.CurrentIndex].operand = operand;
        return this;
    }

    /// <summary>Returns the stack pointer to a previous state.</summary>
    /// <param name="count">Number of index changes to discard.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Return(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            this._indexStack.Pop();
        }

        return this;
    }

    /// <summary>
    ///     Applies the specified action to all occurrences of the <paramref name="pattern"/> within the active
    ///     <see cref="CodeInstruction"/> list.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <param name="action">The action to be applied.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper ForEach(CodeInstruction[] pattern, Action action)
    {
        while (this.TryMoveNext(pattern))
        {
            action.Invoke();
        }

        return this;
    }

    /// <summary>Applies the specified action a species number of times.</summary>
    /// <param name="count">The number of times to repeat.</param>
    /// <param name="action">The action to be applied.</param>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Repeat(int count, Action<int> action)
    {
        for (var i = 0; i < count; i++)
        {
            action.Invoke(i);
        }

        return this;
    }

    /// <summary>Resets the current instance.</summary>
    /// <returns>The <see cref="ILHelper"/> instance.</returns>
    public ILHelper Clear()
    {
        this._indexStack.Clear();
        this._instructions.Clear();
        return this;
    }

    /// <summary>Resets the instance and returns the active <see cref="CodeInstruction"/> list as an enumerable.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> with the contents of the <see cref="CodeInstruction"/> cache.</returns>
    public IEnumerable<CodeInstruction> Flush()
    {
        var result = this._instructions.Clone();
        this.Clear();
        return result.AsEnumerable();
    }

    /// <summary>Gets the corresponding <see cref="CodeInstruction"/> for loading a given integer.</summary>
    /// <param name="num">An integer.</param>
    /// <returns>The correct <see cref="CodeInstruction"/> which loads <paramref name="num"/>.</returns>
    public CodeInstruction LdcFromInt(int num)
    {
        return num switch
        {
            0 => new CodeInstruction(OpCodes.Ldc_I4_0),
            1 => new CodeInstruction(OpCodes.Ldc_I4_1),
            2 => new CodeInstruction(OpCodes.Ldc_I4_2),
            3 => new CodeInstruction(OpCodes.Ldc_I4_3),
            4 => new CodeInstruction(OpCodes.Ldc_I4_4),
            5 => new CodeInstruction(OpCodes.Ldc_I4_5),
            6 => new CodeInstruction(OpCodes.Ldc_I4_6),
            7 => new CodeInstruction(OpCodes.Ldc_I4_7),
            8 => new CodeInstruction(OpCodes.Ldc_I4_8),
            > byte.MaxValue => new CodeInstruction(OpCodes.Ldc_I4, num),
            _ => new CodeInstruction(OpCodes.Ldc_I4_S, num),
        };
    }

    /// <summary>Snitches on other <see cref="HarmonyTranspiler"/>s applied to the target <see cref="Original"/> method.</summary>
    /// <returns>A formatted string listing all transpilers applied to the target method.</returns>
    /// <remarks>
    ///     Inspired by
    ///     <see href="https://github.com/atravita-mods/StardewMods/blob/f450bd2fe72a884e89ca6a06c187605bdb79fa3d/AtraShared/Utils/Extensions/HarmonyExtensions.cs#L46">atravita</see>.
    /// </remarks>
    /// <returns>A formatted <see cref="string"/> revealing the currently applied <see cref="HarmonyTranspiler"/>s to the <see cref="Original"/> method.</returns>
    private string Snitch()
    {
        var sb = new StringBuilder();
        sb.Append("Applied transpilers:");
        var count = 0;
        foreach (var transpiler in this.Original.GetAppliedTranspilers())
        {
            sb.AppendLine().Append($"\t{transpiler.PatchMethod.GetFullName()}");
            count++;
        }

        return count > 0 ? sb.ToString() : string.Empty;
    }

    /// <summary>Attempts to move the stack pointer to the next occurrence of the specified <paramref name="pattern"/>.</summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>
    ///     <see langword="true"/> if a subsequent occurrence of <paramref name="pattern"/> is found, otherwise
    ///     <see langword="false"/>.
    /// </returns>
    private bool TryMoveNext(params CodeInstruction[] pattern)
    {
        var index = this._instructions.IndexOf(pattern, this.CurrentIndex + 1);
        if (index < 0)
        {
            return false;
        }

        this._indexStack.Push(index);
        return true;
    }
}
