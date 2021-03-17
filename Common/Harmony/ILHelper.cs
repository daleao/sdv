using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace TheLion.Common.Harmony
{
	/// <summary>Provides an interface to abstract common transpiler operations.</summary>
	public class ILHelper
	{
		private List<CodeInstruction> _instructionList;
		private List<CodeInstruction> _instructionListBackup;
		private List<CodeInstruction> _buffer;
		private readonly Stack<int> _indexStack;
		private readonly IMonitor _monitor;

		/// <summary>The index currently at the top of the index stack.</summary>
		private int _CurrentIndex
		{
			get
			{
				if (_indexStack == null || _indexStack.Count() < 1)
					throw new IndexOutOfRangeException("The index stack is either null or empty.");
				
				return _indexStack.Peek();
			}
		}

		/// <summary>The index currently at the top of the index stack.</summary>
		private int _LastIndex
		{
			get
			{
				if (_instructionList == null || _instructionList.Count() < 1)
					throw new IndexOutOfRangeException("The active instruction list is either null of empty.");
				
				return _instructionList.Count();
			}
		}

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		public ILHelper(IMonitor monitor)
		{
			_indexStack = new();
			_monitor = monitor;
		}

		/// <summary>Construct an instance.</summary>
		/// <param name="instructions">A list of IL code instructions.</param>
		/// <param name="monitor">Writes messages to the console and log file.</param>
		public ILHelper(IEnumerable<CodeInstruction> instructions, IMonitor monitor)
		{
			_instructionList = instructions.ToList();
			_instructionListBackup = _instructionList.Clone();
			_indexStack = new();
			_indexStack.Push(0);
			_monitor = monitor;
		}

		/// <summary>Attach a new list of code instructions to this instance.</summary>
		/// <param name="instructions">A list of IL code instructions.</param>
		public ILHelper Attach(IEnumerable<CodeInstruction> instructions)
		{
			_instructionList = instructions.ToList();
			_instructionListBackup = _instructionList.Clone();

			if (_indexStack.Count > 0)
			{
				_indexStack.Clear();
			}
			_indexStack.Push(0);

			return this;
		}

		/// <summary>Create an internal copy of the active code instruction list.</summary>
		public ILHelper Backup()
		{
			_instructionListBackup = _instructionList.Clone();
			return this;
		}

		/// <summary>Find the first occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper FindFirst(params CodeInstruction[] pattern)
		{
			int index = _instructionList.IndexOf(pattern);
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the last occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper FindLast(params CodeInstruction[] pattern)
		{
			var reversedInstructions = _instructionList.Clone();
			reversedInstructions.Reverse();

			int index = _instructionList.Count() - reversedInstructions.IndexOf(pattern) - 1;
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the next occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper FindNext(params CodeInstruction[] pattern)
		{
			int index = _instructionList.IndexOf(pattern, start: _CurrentIndex + 1);
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the previous occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper FindPrevious(params CodeInstruction[] pattern)
		{
			var reversedInstructions = _instructionList.Clone();
			reversedInstructions.Reverse();

			int index = _instructionList.Count() - reversedInstructions.IndexOf(pattern, start: _instructionList.Count() - _CurrentIndex - 1) - 1;
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find a specific label in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="label">The label to match.</param>
		public ILHelper FindLabel(Label label, bool fromCurrentIndex = false)
		{
			int index = _instructionList.IndexOf(label, start: fromCurrentIndex ? _CurrentIndex + 1 : 0);
			if (index < 0)
				throw new IndexOutOfRangeException("The label was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the first or next occurrence of the pattern corresponding to `player.professions.Contains()` in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="fromCurrentIndex">Whether to begin search from currently pointed index.</param>
		public ILHelper FindProfessionCheck(int whichProfession, bool fromCurrentIndex = false)
		{
			if (fromCurrentIndex)
			{
				return FindNext(
					new CodeInstruction(OpCodes.Ldfld, operand: AccessTools.Field(typeof(Farmer), nameof(Farmer.professions))),
					_LoadConstantIntIL(whichProfession),
					new CodeInstruction(OpCodes.Callvirt, operand: AccessTools.Method(typeof(NetList<Int32, NetInt>), nameof(NetList<Int32, NetInt>.Contains)))
				);
			}
			
			return FindFirst(
				new CodeInstruction(OpCodes.Ldfld, operand: AccessTools.Field(typeof(Farmer), nameof(Farmer.professions))),
				_LoadConstantIntIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt, operand: AccessTools.Method(typeof(NetList<Int32, NetInt>), nameof(NetList<Int32, NetInt>.Contains)))
			);
		}

		/// <summary>Move the index pointer forward an integer number of steps.</summary>
		/// <param name="steps">Number of steps by which to move the index pointer.</param>
		public ILHelper Advance(int steps = 1)
		{
			if (_CurrentIndex + steps < 0 || _CurrentIndex + steps + 1 > _LastIndex)
				throw new IndexOutOfRangeException("New index is out of range.");

			_indexStack.Push(_CurrentIndex + steps);
			return this;
		}

		/// <summary>Alias for <c>FindNext(pattern)</c>.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper AdvanceUntil(params CodeInstruction[] pattern)
		{
			return FindNext(pattern);
		}

		/// <summary>Alias for <c>FindLabel(label, fromCurrentIndex: true)</c>.</summary>
		/// <param name="label">The label to match.</param>
		public ILHelper AdvanceUntilLabel(Label label)
		{
			return FindLabel(label, fromCurrentIndex: true);
		}

		/// <summary>Move the index pointer backward an integer number of steps.</summary>
		/// <param name="steps">Number of steps by which to move the index pointer.</param>
		public ILHelper Retreat(int steps = 1)
		{
			return Advance(-steps);
		}

		/// <summary>Alias for <c>FindPrevious(pattern)</c>.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper RetreatUntil(params CodeInstruction[] pattern)
		{
			return FindPrevious(pattern);
		}

		/// <summary>Return the index pointer to a previous state.</summary>
		/// <param name="count">Number of index changes to discard.</param>
		public ILHelper Return(int count = 1)
		{
			for (int i = 0; i < count; ++i)
			{
				_indexStack.Pop();
			}
			return this;
		}

		/// <summary>Move the index pointer to a specific index.</summary>
		/// <param name="index">The index to move to.</param>
		public ILHelper GoTo(int index)
		{
			if (index < 0)
				throw new IndexOutOfRangeException("Cannot go to a negative index.");

			if (index > _LastIndex - 1)
				throw new IndexOutOfRangeException("New index is out of range.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Replace the code instruction at the currently pointed index.</summary>
		/// <param name="instruction">The instruction to replace with.</param>
		public ILHelper ReplaceWith(CodeInstruction instruction)
		{
			_instructionList[_CurrentIndex] = instruction;
			return this;
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index.</summary>
		/// <param name="instructions">A sequence of code instructions to insert.</param>
		public ILHelper Insert(params CodeInstruction[] instructions)
		{
			_instructionList.InsertRange(_CurrentIndex, instructions);
			_indexStack.Push(_CurrentIndex + instructions.Count());
			return this;
		}

		/// <summary>Insert any code instructions in the buffer at the currently pointed index.</summary>
		public ILHelper InsertBuffer()
		{
			Insert(_buffer.ToArray());
			return this;
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to test if the local player has a given profession.</summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="branchDestination">The destination to branch to when the check returns false.</param>
		public ILHelper InsertProfessionCheckForLocalPlayer(int whichProfession, Label branchDestination, bool branchIfTrue = false)
		{
			return Insert(
				new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Game1), nameof(Game1.player)).GetGetMethod()),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.professions))),
				_LoadConstantIntIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetList<Int32, NetInt>), nameof(NetList<Int32, NetInt>.Contains))),
				new CodeInstruction(branchIfTrue ? OpCodes.Brtrue_S : OpCodes.Brfalse_S, operand: branchDestination)
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to test if the player at the top of the stack has a given profession.</summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="branchDestination">The destination to branch to when the check returns false.</param>
		public ILHelper InsertProfessionCheckForSpecificPlayer(int whichProfession, Label branchDestination, bool branchIfTrue = false)
		{
			return Insert(
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.professions))),
				_LoadConstantIntIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetList<Int32, NetInt>), nameof(NetList<Int32, NetInt>.Contains))),
				new CodeInstruction(branchIfTrue ? OpCodes.Brtrue_S : OpCodes.Brfalse_S, operand: branchDestination)
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random integer.</summary>
		/// <param name="minValue">The profession id.</param>
		/// <param name="maxValue">The destination to branch to when the check returns false.</param>
		public ILHelper InsertDiceRoll(int minValue, int maxValue)
		{
			return Insert(
				new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Game1), nameof(Game1.random))),
				_LoadConstantIntIL(minValue),
				_LoadConstantIntIL(maxValue + 1),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Random), nameof(Random.Next)))
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random double.</summary>
		public ILHelper InsertDiceRoll()
		{
			return Insert(
				new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Game1), nameof(Game1.random))),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Random), nameof(Random.NextDouble)))
			);
		}

		/// <summary>Remove code instructions starting from the currently pointed index.</summary>
		/// <param name="count">Number of code instructions to remove.</param>
		public ILHelper Remove(int count = 1)
		{
			if (_CurrentIndex + count + 1 > _LastIndex)
				throw new IndexOutOfRangeException("Cannot remove item out of range.");

			_instructionList.RemoveRange(_CurrentIndex, count);
			return this;
		}

		/// <summary>Remove code instructions starting from the currently pointed index until a specific pattern is found.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper RemoveUntil(params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			int endIndex = _indexStack.Pop() + 1;
			int count = endIndex - _CurrentIndex;
			_instructionList.RemoveRange(_CurrentIndex, count);

			return this;
		}

		/// <summary>Remove code instructions starting from the currently pointed index until a specific label is found.</summary>
		/// <param name="label">The label to match.</param>
		public ILHelper RemoveUntilLabel(Label label)
		{
			AdvanceUntilLabel(label);

			int endIndex = _indexStack.Pop() + 1;
			int count = endIndex - _CurrentIndex;
			_instructionList.RemoveRange(_CurrentIndex, count);

			return this;
		}

		/// <summary>Copy code instructions starting from the currently pointed index to the buffer.</summary>
		/// <param name="count">Number of code instructions to copy.</param>
		/// <param name="stripLabels">Whether to remove the labels from the copied instructions.</param>
		/// <param name="advance">Whether to advance the index pointer.</param>
		public ILHelper ToBuffer(int count = 1, bool stripLabels = false, bool advance = false)
		{
			_buffer = _instructionList.GetRange(_CurrentIndex, count).Clone();
			
			if (stripLabels)
				_StripLabelsFromBuffer();

			if (advance)
				_indexStack.Push(_CurrentIndex + count);

			return this;
		}

		/// <summary>Copy code instructions starting from the currently pointed index until a specific pattern is found to the buffer.</summary>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper ToBufferUntil(params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			int endIndex = _indexStack.Pop() + 1;
			int count = endIndex - _CurrentIndex;
			_buffer = _instructionList.GetRange(_CurrentIndex, count).Clone();

			return this;
		}

		/// <summary>Copy code instructions starting from the currently pointed index until a specific pattern is found to the buffer.</summary>
		/// <param name="stripLabels">Whether to remove the labels from the copied instructions.</param>
		/// <param name="advance">Whether to advance the index pointer.</param>
		/// <param name="pattern">A sequence of code instructions to match.</param>
		public ILHelper ToBufferUntil(bool stripLabels, bool advance, params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			int endIndex = _indexStack.Pop() + 1;
			int count = endIndex - _CurrentIndex;
			_buffer = _instructionList.GetRange(_CurrentIndex, count).Clone();

			if (stripLabels)
				_StripLabelsFromBuffer();

			if (advance)
				_indexStack.Push(endIndex);

			return this;
		}

		/// <summary>Get the labels from the code instruction at the currently pointed index.</summary>
		/// <param name="labels">The returned list of label objects.</param>
		public ILHelper GetLabels(out List<Label> labels)
		{
			labels = _instructionList[_CurrentIndex].labels.Clone();
			//if (labels.Count == 0)
			//	throw new ArgumentNullException("Instruction does not have labels.");

			return this;
		}

		/// <summary>Add a single label to the code instruction at the currently pointed index.</summary>
		/// <param name="label">The label to add.</param>
		public ILHelper AddLabel(Label label)
		{
			_instructionList[_CurrentIndex].labels.Add(label);
			return this;
		}

		/// <summary>Add one or more labels to the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A list of labels to add.</param>
		public ILHelper AddLabels(List<Label> labels)
		{
			_instructionList[_CurrentIndex].labels.AddRange(labels);
			return this;
		}

		/// <summary>Set the labels of the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A list of labels.</param>
		public ILHelper SetLabels(List<Label> labels)
		{
			_instructionList[_CurrentIndex].labels = labels;
			return this;
		}

		/// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
		public ILHelper StripLabels()
		{
			_instructionList[_CurrentIndex].labels.Clear();
			return this;
		}

		/// <summary>Return the opcode of the code instruction at the currently pointed index.</summary>
		/// <param name="opcode">The returned opcode.</param>
		public ILHelper GetOpCode(out OpCode opcode)
		{
			opcode = _instructionList[_CurrentIndex].opcode;
			return this;
		}

		/// <summary>Change the opcode of the code instruction at the currently pointed index.</summary>
		/// <param name="opcode">The new opcode.</param>
		public ILHelper SetOpCode(OpCode opcode)
		{
			_instructionList[_CurrentIndex].opcode = opcode;
			return this;
		}

		/// <summary>Return the operand of the code instruction at the currently pointed index.</summary>
		/// <param name="operand">The returned operand.</param>
		public ILHelper GetOperand(out object operand)
		{
			operand = _instructionList[_CurrentIndex].operand;
			return this;
		}

		/// <summary>Change the operand of the code instruction at the currently pointed index.</summary>
		/// <param name="operand">The new operand.</param>
		public ILHelper SetOperand(object operand)
		{
			_instructionList[_CurrentIndex].operand = operand;
			return this;
		}

		/// <summary>Log information to the SMAPI console.</summary>
		/// <param name="text">The message to log.</param>
		public ILHelper Log(string text)
		{
			_monitor.Log(text, LogLevel.Info);
			return this;
		}

		/// <summary>Log a warning to the SMAPI console.</summary>
		/// <param name="text">The warning message.</param>
		public ILHelper Warn(string text)
		{
			_monitor.Log(text, LogLevel.Warn);
			return this;
		}

		/// <summary>Log an error to the SMAPI console.</summary>
		/// <param name="text">The error message.</param>
		public ILHelper Error(string text)
		{
			_monitor.Log(text, LogLevel.Error);
			return this;
		}

		/// <summary>Reset the current instance.</summary>
		public ILHelper Clear()
		{
			_indexStack.Clear();
			_instructionList.Clear();
			return this;
		}

		/// <summary>Restore the active code instruction list to the backed-up state.</summary>
		public ILHelper Restore()
		{
			_indexStack.Clear();
			_instructionList = _instructionListBackup;
			return this;
		}

		/// <summary>Return the active code instruction list as enumerable.</summary>
		public IEnumerable<CodeInstruction> Flush()
		{
			return _instructionList.AsEnumerable();
		}

		/// <summary>Remove any labels from the code instructions currently in the buffer.</summary>
		private void _StripLabelsFromBuffer()
		{
			for (int i = 0; i < _buffer.Count(); ++i)
			{
				_buffer[i].labels.Clear();
			}
		}

		/// <summary>Get the corresponding IL code instruction which loads a given integer.</summary>
		/// <param name="number">An integer.</param>
		private CodeInstruction _LoadConstantIntIL(int number)
		{
			return number switch
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
				_ => new CodeInstruction(OpCodes.Ldc_I4_S, operand: number)
			};
		}
	}
}
