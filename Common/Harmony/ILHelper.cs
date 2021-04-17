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
		private IMonitor _Monitor { get; }

		private List<CodeInstruction> _instructionList;
		private List<CodeInstruction> _instructionListBackup;
		private List<CodeInstruction> _buffer;
		private readonly Stack<int> _indexStack;

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

				return _instructionList.Count() - 1;
			}
		}

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		public ILHelper(IMonitor monitor)
		{
			_indexStack = new();
			_Monitor = monitor;
		}

		/// <summary>Construct an instance.</summary>
		/// <param name="instructions">Enumerable of <see cref="CodeInstruction"/> objects.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		public ILHelper(IEnumerable<CodeInstruction> instructions, IMonitor monitor)
		{
			_instructionList = instructions.ToList();
			_instructionListBackup = _instructionList.Clone();
			_indexStack = new();
			_indexStack.Push(0);
			_Monitor = monitor;
		}

		/// <summary>Attach a new list of code instructions to this instance.</summary>
		/// <param name="instructions">Enumerable of <see cref="CodeInstruction"/> objects.</param>
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
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper FindFirst(params CodeInstruction[] pattern)
		{
			var index = _instructionList.IndexOf(pattern);
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the last occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper FindLast(params CodeInstruction[] pattern)
		{
			var reversedInstructions = _instructionList.Clone();
			reversedInstructions.Reverse();

			var index = _instructionList.Count() - reversedInstructions.IndexOf(pattern) - 1;
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the next occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper FindNext(params CodeInstruction[] pattern)
		{
			var index = _instructionList.IndexOf(pattern, start: _CurrentIndex + 1);
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the previous occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper FindPrevious(params CodeInstruction[] pattern)
		{
			var reversedInstructions = _instructionList.Clone();
			reversedInstructions.Reverse();

			var index = _instructionList.Count() - reversedInstructions.IndexOf(pattern, start: _instructionList.Count() - _CurrentIndex - 1) - 1;
			if (index < 0)
				throw new IndexOutOfRangeException("The instruction pattern was not found.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find a specific label in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="label">The <see cref="Label"/> object to match.</param>
		public ILHelper FindLabel(Label label, bool fromCurrentIndex = false)
		{
			var index = _instructionList.IndexOf(label, start: fromCurrentIndex ? _CurrentIndex + 1 : 0);
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
			if (_CurrentIndex + steps < 0 || _CurrentIndex + steps > _LastIndex)
				throw new IndexOutOfRangeException("New index is out of range.");

			_indexStack.Push(_CurrentIndex + steps);
			return this;
		}

		/// <summary>Alias for <see cref="FindNext(CodeInstruction[])"/>.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper AdvanceUntil(params CodeInstruction[] pattern)
		{
			return FindNext(pattern);
		}

		/// <summary>Alias for <see cref="FindLabel(Label, bool)"/> with parameter <c>fromCurrentIndex = true</c>.</summary>
		/// <param name="label">The <see cref="Label"/> object to match.</param>
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

		/// <summary>Alias for <see cref="FindPrevious(CodeInstruction[])"/>.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper RetreatUntil(params CodeInstruction[] pattern)
		{
			return FindPrevious(pattern);
		}

		/// <summary>Return the index pointer to a previous state.</summary>
		/// <param name="count">Number of index changes to discard.</param>
		public ILHelper Return(int count = 1)
		{
			for (var i = 0; i < count; ++i)
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

			if (index > _LastIndex)
				throw new IndexOutOfRangeException("New index is out of range.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Replace the code instruction at the currently pointed index.</summary>
		/// <param name="instruction">The <see cref="CodeInstruction"/> object to replace with.</param>
		public ILHelper ReplaceWith(CodeInstruction instruction)
		{
			_instructionList[_CurrentIndex] = instruction;
			return this;
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index.</summary>
		/// <param name="instructions">Sequence of <see cref="CodeInstruction"/> objects to insert.</param>
		public ILHelper Insert(params CodeInstruction[] instructions)
		{
			_instructionList.InsertRange(_CurrentIndex, instructions);
			_indexStack.Push(_CurrentIndex + instructions.Count());
			return this;
		}

		/// <summary>Insert the buffer contents at the currently pointed index.</summary>
		public ILHelper InsertBuffer()
		{
			Insert(_buffer.Clone().ToArray());
			return this;
		}

		/// <summary>Insert a subset of the buffer contents at the currently pointed index.</summary>
		/// <param name="index">The starting index.</param>
		/// <param name="length">The subset length.</param>
		public ILHelper InsertBuffer(int index, int length)
		{
			Insert(_buffer.Clone().ToArray().SubArray(index, length));
			return this;
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to test if the local player has a given profession.</summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="branchDestination">The destination <see cref="Label"/> to branch to when the check returns false.</param>
		public ILHelper InsertProfessionCheckForLocalPlayer(int whichProfession, Label branchDestination, bool useBrtrue = false, bool useLongFormBranch = false)
		{
			OpCode branchOpCode;
			if (useBrtrue && useLongFormBranch) branchOpCode = OpCodes.Brtrue;
			else if (useBrtrue) branchOpCode = OpCodes.Brtrue_S;
			else if (useLongFormBranch) branchOpCode = OpCodes.Brfalse;
			else branchOpCode = OpCodes.Brfalse_S;

			return Insert(
				new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Game1), nameof(Game1.player)).GetGetMethod()),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.professions))),
				_LoadConstantIntIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetList<Int32, NetInt>), nameof(NetList<Int32, NetInt>.Contains))),
				new CodeInstruction(branchOpCode, operand: branchDestination)
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to test if the player at the top of the stack has a given profession.</summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="branchDestination">The destination <see cref="Label"/> to branch to when the check returns false.</param>
		public ILHelper InsertProfessionCheckForPlayerOnStack(int whichProfession, Label branchDestination, bool useBrtrue = false, bool useLongFormBranch = false)
		{
			OpCode branchOpCode;
			if (useBrtrue && useLongFormBranch) branchOpCode = OpCodes.Brtrue;
			else if (useBrtrue) branchOpCode = OpCodes.Brtrue_S;
			else if (useLongFormBranch) branchOpCode = OpCodes.Brfalse;
			else branchOpCode = OpCodes.Brfalse_S;

			return Insert(
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.professions))),
				_LoadConstantIntIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NetList<Int32, NetInt>), nameof(NetList<Int32, NetInt>.Contains))),
				new CodeInstruction(branchOpCode, operand: branchDestination)
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random integer.</summary>
		/// <param name="minValue">The lower limit, inclusive.</param>
		/// <param name="maxValue">The upper limit, inclusive.</param>
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
			if (_CurrentIndex + count > _LastIndex)
				throw new IndexOutOfRangeException("Cannot remove item out of range.");

			_instructionList.RemoveRange(_CurrentIndex, count);
			return this;
		}

		/// <summary>Remove code instructions starting from the currently pointed index until a specific pattern is found.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper RemoveUntil(params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - _CurrentIndex;
			_instructionList.RemoveRange(_CurrentIndex, count);

			return this;
		}

		/// <summary>Remove code instructions starting from the currently pointed index until a specific label is found.</summary>
		/// <param name="label">The <see cref="Label"/> object to match.</param>
		public ILHelper RemoveUntilLabel(Label label)
		{
			AdvanceUntilLabel(label);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - _CurrentIndex;
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
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper ToBufferUntil(params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - _CurrentIndex;
			_buffer = _instructionList.GetRange(_CurrentIndex, count).Clone();

			return this;
		}

		/// <summary>Copy code instructions starting from the currently pointed index until a specific pattern is found to the buffer.</summary>
		/// <param name="stripLabels">Whether to remove the labels from the copied instructions.</param>
		/// <param name="advance">Whether to advance the index pointer.</param>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction"/> objects to match.</param>
		public ILHelper ToBufferUntil(bool stripLabels, bool advance, params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - _CurrentIndex;
			_buffer = _instructionList.GetRange(_CurrentIndex, count).Clone();

			if (stripLabels)
				_StripLabelsFromBuffer();

			if (advance)
				_indexStack.Push(endIndex);

			return this;
		}

		/// <summary>Get the labels from the code instruction at the currently pointed index.</summary>
		/// <param name="labels">The returned list of <see cref="Label"/> objects.</param>
		public ILHelper GetLabels(out Label[] labels)
		{
			labels = _instructionList[_CurrentIndex].labels.Clone().ToArray();
			return this;
		}

		/// <summary>Add one or more labels to the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A sequence of <see cref="Label"/> objects to add.</param>
		public ILHelper AddLabels(params Label[] labels)
		{
			_instructionList[_CurrentIndex].labels.AddRange(labels);
			return this;
		}

		/// <summary>Set the labels of the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A list of <see cref="Label"/> objects.</param>
		public ILHelper SetLabels(params Label[] labels)
		{
			_instructionList[_CurrentIndex].labels = labels.ToList();
			return this;
		}

		/// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
		public ILHelper StripLabels()
		{
			_instructionList[_CurrentIndex].labels.Clear();
			return this;
		}

		/// <summary>Return the opcode of the code instruction at the currently pointed index.</summary>
		/// <param name="opcode">The returned <see cref="OpCode"/> object.</param>
		public ILHelper GetOpCode(out OpCode opcode)
		{
			opcode = _instructionList[_CurrentIndex].opcode;
			return this;
		}

		/// <summary>Change the opcode of the code instruction at the currently pointed index.</summary>
		/// <param name="opcode">The new <see cref="OpCode"/> object.</param>
		public ILHelper SetOpCode(OpCode opcode)
		{
			_instructionList[_CurrentIndex].opcode = opcode;
			return this;
		}

		/// <summary>Return the operand of the code instruction at the currently pointed index.</summary>
		/// <param name="operand">The returned operand <see cref="object"/>.</param>
		public ILHelper GetOperand(out object operand)
		{
			operand = _instructionList[_CurrentIndex].operand;
			return this;
		}

		/// <summary>Change the operand of the code instruction at the currently pointed index.</summary>
		/// <param name="operand">The new <see cref="object"/> operand.</param>
		public ILHelper SetOperand(object operand)
		{
			_instructionList[_CurrentIndex].operand = operand;
			return this;
		}

		/// <summary>Log information to the SMAPI console.</summary>
		/// <param name="text">The message to log.</param>
		public ILHelper Trace(string text)
		{
			_Monitor.Log(text, LogLevel.Trace);
			return this;
		}

		/// <summary>Log a warning to the SMAPI console.</summary>
		/// <param name="text">The warning message.</param>
		public ILHelper Warn(string text)
		{
			_Monitor.Log(text, LogLevel.Warn);
			return this;
		}

		/// <summary>Log an error to the SMAPI console.</summary>
		/// <param name="text">The error message.</param>
		public ILHelper Error(string text)
		{
			_Monitor.Log(text, LogLevel.Error);
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

		/// <summary>Reset the instance and return the active code instruction list as enumerable.</summary>
		public IEnumerable<CodeInstruction> Flush()
		{
			var result = _instructionList.Clone();
			Clear();
			return result.AsEnumerable();
		}

		/// <summary>Remove any labels from code instructions currently in the buffer.</summary>
		private void _StripLabelsFromBuffer()
		{
			for (var i = 0; i < _buffer.Count(); ++i)
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