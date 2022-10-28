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
public sealed class IlHelper
{
    private readonly Stack<int> _indexStack = new();

    /// <summary>Initializes a new instance of the <see cref="IlHelper"/> class.</summary>
    /// <param name="original">A <see cref="MethodBase"/> representation of the original method.</param>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to be modified.</param>
    public IlHelper(MethodBase original, IEnumerable<CodeInstruction> instructions)
    {
        this.Original = original;
        this.Instructions = instructions.ToList();
        this.Locals = this.Instructions
            .Where(instruction => (instruction.IsLdloc() || instruction.IsStloc()) && instruction.operand is not null)
            .Select(instruction => (LocalBuilder)instruction.operand)
            .ToHashSet()
            .ToDictionary(lb => lb.LocalIndex, lb => lb);

        this._indexStack.Push(0);
    }

    /// <summary>Gets metadata about the original target method.</summary>
    public MethodBase Original { get; }

    /// <summary>Gets the current list of <see cref="CodeInstruction"/>s that will eventually replace the target method.</summary>
    public List<CodeInstruction> Instructions { get; }

    /// <summary>Gets a look-up table for easy indexing of <see cref="LocalBuilder"/> objects by their corresponding local index.</summary>
    public Dictionary<int, LocalBuilder> Locals { get; }

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
            if (this.Instructions.Count == 0)
            {
                ThrowHelper.ThrowInvalidOperationException(
                    "Tried to access the instruction list while it was null or empty.");
            }

            return this.Instructions.Count - 1;
        }
    }

    /// <summary>
    ///     Finds the first occurrence of the specified <paramref name="pattern"/> in the active
    ///     <see cref="CodeInstruction"/> list and moves the index pointer to it.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper FindFirst(params CodeInstruction[] pattern)
    {
        var index = this.Instructions.IndexOf(pattern);
        if (index < 0)
        {
            ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>
    ///     Finds the last occurrence of the specified <paramref name="pattern"/> in the active
    ///     <see cref="CodeInstruction"/> list and moves the index pointer to it.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper FindLast(params CodeInstruction[] pattern)
    {
        var reversedInstructions = this.Instructions.Clone();
        reversedInstructions.Reverse();

        var index = this.Instructions.Count - reversedInstructions.IndexOf(pattern.Reverse().ToArray()) -
                    pattern.Length;
        if (index < 0)
        {
            ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>
    ///     Finds the next occurrence of the specified <paramref name="pattern"/> in the active
    ///     <see cref="CodeInstruction"/> list and moves the index pointer to it.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper FindNext(params CodeInstruction[] pattern)
    {
        var index = this.Instructions.IndexOf(pattern, this.CurrentIndex + 1);
        if (index < 0)
        {
            ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>
    ///     Find the previous occurrence of the specified <paramref name="pattern"/> in the active
    ///     <see cref="CodeInstruction"/> list and move the index pointer to it.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper FindPrevious(params CodeInstruction[] pattern)
    {
        var reversedInstructions = this.Instructions.Clone();
        reversedInstructions.Reverse();

        var index = this.Instructions.Count - pattern.Length - reversedInstructions.IndexOf(
            pattern.Reverse().ToArray(), this.Instructions.Count - this.CurrentIndex);
        if (index >= this.Instructions.Count)
        {
            ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>
    ///     Finds the specified <paramref name="label"/> in the active <see cref="CodeInstruction"/> list and moves the
    ///     index pointer to it.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> object to match.</param>
    /// <param name="fromCurrentIndex">Whether to begin search from the currently pointed index.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper FindLabel(Label label, bool fromCurrentIndex = false)
    {
        var index = this.Instructions.IndexOf(label, fromCurrentIndex ? this.CurrentIndex + 1 : 0);
        if (index < 0)
        {
            ThrowHelperExtensions.ThrowLabelNotFoundException(label, this.Original, this.Snitch);
        }

        this._indexStack.Push(index);
        return this;
    }

    /// <summary>Moves the index pointer forward an integer number of <paramref name="steps"/>.</summary>
    /// <param name="steps">Number of steps by which to move the index pointer.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper Advance(int steps = 1)
    {
        if (this.CurrentIndex + steps < 0 || this.CurrentIndex + steps > this.LastIndex)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("New index is out of range.");
        }

        this._indexStack.Push(this.CurrentIndex + steps);
        return this;
    }

    /// <summary>Alias for <see cref="FindNext(CodeInstruction[])"/>.</summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper AdvanceUntil(params CodeInstruction[] pattern)
    {
        return this.FindNext(pattern);
    }

    /// <summary>Moves the index pointer backward an integer number of <paramref name="steps"/>.</summary>
    /// <param name="steps">Number of steps by which to move the index pointer.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper Retreat(int steps = 1)
    {
        return this.Advance(-steps);
    }

    /// <summary>Alias for <see cref="FindPrevious(CodeInstruction[])"/>.</summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper RetreatUntil(params CodeInstruction[] pattern)
    {
        return this.FindPrevious(pattern);
    }

    /// <summary>Inserts the given <paramref name="instructions"/> at the currently pointed index.</summary>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to insert.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    /// <remarks>
    ///     The instruction at the current address is pushed forward, such that the index pointer continues to point to
    ///     the same instruction after insertion.
    /// </remarks>
    public IlHelper InsertInstructions(params CodeInstruction[] instructions)
    {
        this.Instructions.InsertRange(this.CurrentIndex, instructions);
        this._indexStack.Push(this.CurrentIndex + instructions.Length);
        return this;
    }

    /// <summary>
    ///     Inserts the given <paramref name="instructions"/> at the currently pointed index and adds the specified
    ///     <see cref="Label"/>s to the first of those <paramref name="instructions"/>.
    /// </summary>
    /// <param name="labels">Some <see cref="CodeInstruction"/>s to add at the start of the insertion.</param>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to insert.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    /// <remarks>
    ///     The instruction at the current address is pushed forward, such that the index pointer continues to point to
    ///     the same instruction after insertion.
    /// </remarks>
    public IlHelper InsertWithLabels(Label[] labels, params CodeInstruction[] instructions)
    {
        instructions[0].labels.AddRange(labels);
        this.Instructions.InsertRange(this.CurrentIndex, instructions);
        this._indexStack.Push(this.CurrentIndex + instructions.Length);
        return this;
    }

    /// <summary>Adds the given <paramref name="instructions"/> to the end of the active <see cref="CodeInstruction"/> list.</summary>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to add.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    /// <remarks>The index pointer is moved to the first instruction in the added sequence.</remarks>
    public IlHelper AddInstructions(params CodeInstruction[] instructions)
    {
        this.Instructions.AddRange(instructions);
        this._indexStack.Push(this.LastIndex - instructions.Length);
        return this;
    }

    /// <summary>
    ///     Adds the given <paramref name="instructions"/> to the end of the active <see cref="CodeInstruction"/> list
    ///     and adds the specified <see cref="Label"/>s to the first of those <paramref name="instructions"/>.
    /// </summary>
    /// <param name="labels">Some <see cref="Label"/>s to add at the start of the insertion.</param>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to add.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    /// <remarks>The index pointer is moved to the first instruction in the added sequence.</remarks>
    public IlHelper AddWithLabels(Label[] labels, params CodeInstruction[] instructions)
    {
        instructions[0].labels.AddRange(labels);
        this.Instructions.AddRange(instructions);
        this._indexStack.Push(this.LastIndex - instructions.Length);
        return this;
    }

    /// <summary>
    ///     Gets a copy of the next <paramref name="count"/> <see cref="CodeInstruction"/>s, starting from the currently
    ///     pointed index.
    /// </summary>
    /// <param name="got">The got code instructions.</param>
    /// <param name="count">Number of code instructions to get.</param>
    /// <param name="removeLabels">Whether to remove the labels of the copied <see cref="CodeInstruction"/>s.</param>
    /// <param name="advance">Whether to advance the index pointer.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper GetInstructions(
        out CodeInstruction[] got, int count = 1, bool removeLabels = false, bool advance = false)
    {
        got = this.Instructions.GetRange(this.CurrentIndex, count).Clone().ToArray();
        if (removeLabels)
        {
            foreach (var insn in got)
            {
                insn.labels.Clear();
            }
        }

        if (advance)
        {
            this._indexStack.Push(this._indexStack.Peek() + count);
        }

        return this;
    }

    /// <summary>
    ///     Gets a copy of the <see cref="CodeInstruction"/>s starting from the currently pointed index up to and
    ///     including the first instruction in the specified <paramref name="pattern"/>.
    /// </summary>
    /// <param name="got">The got code instructions.</param>
    /// <param name="removeLabels">Whether to remove the labels of the copied <see cref="CodeInstruction"/>s.</param>
    /// <param name="advance">Whether to advance the index pointer.</param>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper GetInstructionsUntil(
        out CodeInstruction[] got, bool removeLabels = false, bool advance = false, params CodeInstruction[] pattern)
    {
        this.AdvanceUntil(pattern);

        var endIndex = this._indexStack.Pop() + 1;
        var count = endIndex - this.CurrentIndex;
        got = this.Instructions.GetRange(this.CurrentIndex, count).Clone().ToArray();
        if (removeLabels)
        {
            foreach (var instruction in got)
            {
                instruction.labels.Clear();
            }
        }

        if (advance)
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
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper RemoveInstructions(int count = 1)
    {
        if (this.CurrentIndex + count > this.LastIndex)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("Can't remove item out of range.");
        }

        this.Instructions.RemoveRange(this.CurrentIndex, count);
        return this;
    }

    /// <summary>
    ///     Removes <see cref="CodeInstruction"/>s starting from the currently pointed index up to and including the
    ///     first instruction in the specified <paramref name="pattern"/>.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper RemoveInstructionsUntil(params CodeInstruction[] pattern)
    {
        this.AdvanceUntil(pattern);
        var endIndex = this._indexStack.Pop() + 1;
        var count = endIndex - this.CurrentIndex;
        this.Instructions.RemoveRange(this.CurrentIndex, count);
        return this;
    }

    /// <summary>Replaces the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="instruction">The <see cref="CodeInstruction"/> to replace with.</param>
    /// <param name="preserveLabels">Whether to preserve the labels at the current <see cref="CodeInstruction"/> before replacement.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper ReplaceInstructionWith(CodeInstruction instruction, bool preserveLabels = false)
    {
        if (preserveLabels)
        {
            instruction.labels = this.Instructions[this.CurrentIndex].labels;
        }

        this.Instructions[this.CurrentIndex] = instruction;
        return this;
    }

    /// <summary>Adds one or more <see cref="Label"/>s to the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="labels">Some of <see cref="Label"/>s to add.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper AddLabels(params Label[] labels)
    {
        this.Instructions[this.CurrentIndex].labels.AddRange(labels);
        return this;
    }

    /// <summary>Removes all <see cref="Label"/>s from the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper RemoveLabels()
    {
        this.Instructions[this.CurrentIndex].labels.Clear();
        return this;
    }

    /// <summary>
    ///     Removes the specified <see cref="Label"/>s from the <see cref="CodeInstruction"/> at the currently pointed
    ///     index.
    /// </summary>
    /// <param name="labels">The <see cref="Label"/>s to remove.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper RemoveLabels(params Label[] labels)
    {
        labels.ForEach(l => this.Instructions[this.CurrentIndex].labels.Remove(l));
        return this;
    }

    /// <summary>Replaces the <see cref="Label"/>s of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="labels">The new <see cref="Label"/>s to set.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper SetLabels(params Label[] labels)
    {
        this.Instructions[this.CurrentIndex].labels = labels.ToList();
        return this;
    }

    /// <summary>
    ///     Gets a copy of the <see cref="Label"/>s from the <see cref="CodeInstruction"/> at the currently pointed
    ///     index.
    /// </summary>
    /// <param name="labels">The copied <see cref="Label"/>s.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper GetLabels(out Label[] labels)
    {
        labels = this.Instructions[this.CurrentIndex].labels.ToArray();
        return this;
    }

    /// <summary>
    ///     Removes all <see cref="Label"/>s from the <see cref="CodeInstruction"/>s at the currently pointed index and
    ///     returns a reference to those <see cref="Label"/>s.
    /// </summary>
    /// <param name="labels">The removed <see cref="Label"/>s.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper StripLabels(out Label[] labels)
    {
        this.GetLabels(out labels);
        return this.RemoveLabels();
    }

    /// <summary>Returns the <see cref="OpCode"/> of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="opcode">The returned <see cref="OpCode"/>.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper GetOpCode(out OpCode opcode)
    {
        opcode = this.Instructions[this.CurrentIndex].opcode;
        return this;
    }

    /// <summary>Changes the <see cref="OpCode"/> of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="opcode">The new <see cref="OpCode"/> to replace with.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper SetOpCode(OpCode opcode)
    {
        this.Instructions[this.CurrentIndex].opcode = opcode;
        return this;
    }

    /// <summary>Returns the operand of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="operand">The returned operand.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper GetOperand(out object operand)
    {
        operand = this.Instructions[this.CurrentIndex].operand;
        return this;
    }

    /// <summary>Changes the operand of the <see cref="CodeInstruction"/> at the currently pointed index.</summary>
    /// <param name="operand">The new operand to replace with.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper SetOperand(object operand)
    {
        this.Instructions[this.CurrentIndex].operand = operand;
        return this;
    }

    /// <summary>Returns the index pointer to a previous state.</summary>
    /// <param name="count">Number of index changes to discard.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper Return(int count = 1)
    {
        for (var i = 0; i < count; ++i)
        {
            this._indexStack.Pop();
        }

        return this;
    }

    /// <summary>Moves the index pointer to the specific <paramref name="index"/>.</summary>
    /// <param name="index">The index to move to.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper GoTo(int index)
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

    /// <summary>
    ///     Applies the specified action to all occurrences of the <paramref name="pattern"/> within the active
    ///     <see cref="CodeInstruction"/> list.
    /// </summary>
    /// <param name="pattern">A pattern of <see cref="CodeInstruction"/>s to match.</param>
    /// <param name="action">The action to be applied.</param>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper ForEach(CodeInstruction[] pattern, Action action)
    {
        while (this.TryMoveNext(pattern))
        {
            action.Invoke();
        }

        return this;
    }

    /// <summary>Resets the current instance.</summary>
    /// <returns>The <see cref="IlHelper"/> instance.</returns>
    public IlHelper Clear()
    {
        this._indexStack.Clear();
        this.Instructions.Clear();
        return this;
    }

    /// <summary>Resets the instance and returns the active <see cref="CodeInstruction"/> list as an enumerable.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> with the contents of the <see cref="CodeInstruction"/> cache.</returns>
    public IEnumerable<CodeInstruction> Flush()
    {
        var result = this.Instructions.Clone();
        this.Clear();
        return result.AsEnumerable();
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
            ++count;
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
        var index = this.Instructions.IndexOf(pattern, this.CurrentIndex + 1);
        if (index < 0)
        {
            return false;
        }

        this._indexStack.Push(index);
        return true;
    }
}
