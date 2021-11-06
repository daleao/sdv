using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Netcode;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Common.Harmony
{
	/// <summary>Provides an interface for abstracting common transpiler operations.</summary>
	public class ILHelper
	{
		private readonly string _exportDir;
		private readonly Stack<int> _indexStack;
		private readonly bool _shouldExport;
		private List<CodeInstruction> _instructionBuffer;
		private List<CodeInstruction> _instructionList;
		private MethodBase _original;

		/// <summary>Construct an instance.</summary>
		/// <param name="enableExport">Whether the instruction list should be saved to disk in case an error is thrown.</param>
		/// <param name="path">The root path where instruction lists will be saved.</param>
		public ILHelper(MethodBase original, IEnumerable<CodeInstruction> instructions, bool enableExport = false,
			string path = "")
		{
			_indexStack = new();
			Attach(original, instructions);
			_shouldExport = enableExport;
			_exportDir = Path.Combine(path, "exports");
		}

		/// <summary>The index currently at the top of the index stack.</summary>
		public int CurrentIndex
		{
			get
			{
				if (_indexStack is null || !_indexStack.Any())
					throw new IndexOutOfRangeException("The index stack is either null or empty.");

				return _indexStack.Peek();
			}
		}

		/// <summary>The index of the last <see cref="CodeInstruction" /> in the current instruction list.</summary>
		public int LastIndex
		{
			get
			{
				if (_instructionList is null || !_instructionList.Any())
					throw new IndexOutOfRangeException("The active instruction list is either null or empty.");

				return _instructionList.Count - 1;
			}
		}

		/// <summary>Attach a new list of code instructions to this instance.</summary>
		/// <param name="original"><see cref="MethodBase" /> representation of the original method.</param>
		/// <param name="instructions">Collection of <see cref="CodeInstruction" /> objects.</param>
		public ILHelper Attach(MethodBase original, IEnumerable<CodeInstruction> instructions)
		{
			_original = original;
			_instructionList = instructions.ToList();

			if (_indexStack.Count > 0) _indexStack.Clear();
			_indexStack.Push(0);

			return this;
		}

		/// <summary>Find the first occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper FindFirst(params CodeInstruction[] pattern)
		{
			var index = _instructionList.IndexOf(pattern);
			if (index < 0)
			{
				if (_shouldExport) Export(pattern.ToList());
				throw new IndexOutOfRangeException($"Couldn't find instruction pattern {pattern}.");
			}

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the last occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper FindLast(params CodeInstruction[] pattern)
		{
			var reversedInstructions = _instructionList.Clone();
			reversedInstructions.Reverse();

			var index = _instructionList.Count - reversedInstructions.IndexOf(pattern.Reverse().ToArray()) - 1;
			if (index < 0)
			{
				if (_shouldExport) Export(pattern.ToList());
				throw new IndexOutOfRangeException($"Couldn't find instruction pattern {pattern}.");
			}

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find the next occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper FindNext(params CodeInstruction[] pattern)
		{
			var index = _instructionList.IndexOf(pattern, CurrentIndex + 1);
			if (index < 0)
			{
				if (_shouldExport) Export(pattern.ToList());
				throw new IndexOutOfRangeException($"Couldn't find instruction pattern {pattern}.");
			}

			_indexStack.Push(index);
			return this;
		}

		/// <summary>
		///     Find the previous occurrence of a pattern in the active code instruction list and move the index pointer to
		///     it.
		/// </summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper FindPrevious(params CodeInstruction[] pattern)
		{
			var reversedInstructions = _instructionList.Clone();
			reversedInstructions.Reverse();

			var index = _instructionList.Count -
			            reversedInstructions.IndexOf(pattern, _instructionList.Count - CurrentIndex - 1) - 1;
			if (index < 0)
			{
				if (_shouldExport) Export(pattern.ToList());
				throw new IndexOutOfRangeException($"Couldn't find instruction pattern {pattern}.");
			}

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Find a specific label in the active code instruction list and move the index pointer to it.</summary>
		/// <param name="label">The <see cref="Label" /> object to match.</param>
		/// <param name="fromCurrentIndex">Whether to begin search from the currently pointed index.</param>
		public ILHelper FindLabel(Label label, bool fromCurrentIndex = false)
		{
			var index = _instructionList.IndexOf(label, fromCurrentIndex ? CurrentIndex + 1 : 0);
			if (index < 0)
			{
				if (_shouldExport) Export(label);
				throw new IndexOutOfRangeException($"Couldn't find label {label}.");
			}

			_indexStack.Push(index);
			return this;
		}

		/// <summary>
		///     Find the first or next occurrence of the pattern corresponding to `player.professions.Contains()` in the
		///     active code instruction list and move the index pointer to it.
		/// </summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="fromCurrentIndex">Whether to begin search from currently pointed index.</param>
		public ILHelper FindProfessionCheck(int whichProfession, bool fromCurrentIndex = false)
		{
			if (fromCurrentIndex)
				return FindNext(
					new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
					LoadConstantIntegerIL(whichProfession),
					new CodeInstruction(OpCodes.Callvirt,
						typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains)))
				);

			return FindFirst(
				new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
				LoadConstantIntegerIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt,
					typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains)))
			);
		}

		/// <summary>Move the index pointer forward an integer number of steps.</summary>
		/// <param name="steps">Number of steps by which to move the index pointer.</param>
		public ILHelper Advance(int steps = 1)
		{
			if (CurrentIndex + steps < 0 || CurrentIndex + steps > LastIndex)
				throw new IndexOutOfRangeException("New index is out of range.");

			_indexStack.Push(CurrentIndex + steps);
			return this;
		}

		/// <summary>Alias for <see cref="FindNext(CodeInstruction[])" />.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper AdvanceUntil(params CodeInstruction[] pattern)
		{
			return FindNext(pattern);
		}

		/// <summary>Alias for <see cref="FindLabel(Label, bool)" /> with parameter <c>fromCurrentIndex = true</c>.</summary>
		/// <param name="label">The <see cref="Label" /> object to match.</param>
		public ILHelper AdvanceUntilLabel(Label label)
		{
			return FindLabel(label, true);
		}

		/// <summary>Move the index pointer backward an integer number of steps.</summary>
		/// <param name="steps">Number of steps by which to move the index pointer.</param>
		public ILHelper Retreat(int steps = 1)
		{
			return Advance(-steps);
		}

		/// <summary>Alias for <see cref="FindPrevious(CodeInstruction[])" />.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper RetreatUntil(params CodeInstruction[] pattern)
		{
			return FindPrevious(pattern);
		}

		/// <summary>Return the index pointer to a previous state.</summary>
		/// <param name="count">Number of index changes to discard.</param>
		public ILHelper Return(int count = 1)
		{
			for (var i = 0; i < count; ++i) _indexStack.Pop();
			return this;
		}

		/// <summary>Move the index pointer to a specific index.</summary>
		/// <param name="index">The index to move to.</param>
		public ILHelper GoTo(int index)
		{
			if (index < 0) throw new IndexOutOfRangeException("Can't go to a negative index.");

			if (index > LastIndex) throw new IndexOutOfRangeException("New index is out of range.");

			_indexStack.Push(index);
			return this;
		}

		/// <summary>Move the index pointer to index zero.</summary>
		public ILHelper ReturnToFirst()
		{
			return GoTo(0);
		}

		/// <summary>Move the index pointer to the last index.</summary>
		public ILHelper AdvanceToLast()
		{
			return GoTo(LastIndex);
		}

		/// <summary>Replace the code instruction at the currently pointed index.</summary>
		/// <param name="instruction">The <see cref="CodeInstruction" /> object to replace with.</param>
		public ILHelper ReplaceWith(CodeInstruction instruction)
		{
			_instructionList[CurrentIndex] = instruction;
			return this;
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index.</summary>
		/// <param name="instructions">Sequence of <see cref="CodeInstruction" /> objects to insert.</param>
		/// <remarks>
		///     The instruction originally at this location is pushed forward. After insertion, the index pointer still points
		///     to this same instruction.
		/// </remarks>
		public ILHelper Insert(params CodeInstruction[] instructions)
		{
			_instructionList.InsertRange(CurrentIndex, instructions);
			_indexStack.Push(CurrentIndex + instructions.Length);
			return this;
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index.</summary>
		/// <param name="instructions">Sequence of <see cref="CodeInstruction" /> objects to insert.</param>
		/// <remarks>
		///     The instruction originally at this location is pushed forward. After insertion, the index pointer still points
		///     to this same instruction.
		/// </remarks>
		public ILHelper Insert(ICollection<CodeInstruction> instructions)
		{
			_instructionList.InsertRange(CurrentIndex, instructions);
			_indexStack.Push(CurrentIndex + instructions.Count);
			return this;
		}

		/// <summary>Insert the buffer contents at the currently pointed index.</summary>
		public ILHelper InsertBuffer()
		{
			Insert(_instructionBuffer.Clone());
			return this;
		}

		/// <summary>Insert a subset of the buffer contents at the currently pointed index.</summary>
		/// <param name="index">The starting index.</param>
		/// <param name="length">The subset length.</param>
		public ILHelper InsertBuffer(int index, int length)
		{
			Insert(_instructionBuffer.Clone().GetRange(index, length));
			return this;
		}

		/// <summary>
		///     Insert a sequence of code instructions at the currently pointed index to test if the local player has a given
		///     profession.
		/// </summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="branchDestination">The destination <see cref="Label" /> to branch to when the check returns false.</param>
		/// <param name="useBrtrue">Whether to end on a true-case branch isntead of default false-case branch.</param>
		/// <param name="useLongFormBranch">Whether to use a long-form branch instead of default short-form branch.</param>
		public ILHelper InsertProfessionCheckForLocalPlayer(int whichProfession, Label branchDestination,
			bool useBrtrue = false, bool useLongFormBranch = false)
		{
			var branchOpCode = useBrtrue switch
			{
				true when useLongFormBranch => OpCodes.Brtrue,
				true => OpCodes.Brtrue_S,
				_ => useLongFormBranch ? OpCodes.Brfalse : OpCodes.Brfalse_S
			};

			return Insert(
				new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
				new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
				LoadConstantIntegerIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt,
					typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains))),
				new CodeInstruction(branchOpCode, branchDestination)
			);
		}

		/// <summary>
		///     Insert a sequence of code instructions at the currently pointed index to test if the player at the top of the
		///     stack has a given profession.
		/// </summary>
		/// <param name="whichProfession">The profession id.</param>
		/// <param name="branchDestination">The destination <see cref="Label" /> to branch to when the check returns false.</param>
		/// <param name="useBrtrue">Whether to end on a true-case branch isntead of default false-case branch.</param>
		/// <param name="useLongFormBranch">Whether to use a long-form branch instead of default short-form branch.</param>
		public ILHelper InsertProfessionCheckForPlayerOnStack(int whichProfession, Label branchDestination,
			bool useBrtrue = false, bool useLongFormBranch = false)
		{
			var branchOpCode = useBrtrue switch
			{
				true when useLongFormBranch => OpCodes.Brtrue,
				true => OpCodes.Brtrue_S,
				_ => useLongFormBranch ? OpCodes.Brfalse : OpCodes.Brfalse_S
			};

			return Insert(
				new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
				LoadConstantIntegerIL(whichProfession),
				new CodeInstruction(OpCodes.Callvirt,
					typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains))),
				new CodeInstruction(branchOpCode, branchDestination)
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random double.</summary>
		public ILHelper InsertDiceRoll()
		{
			return Insert(
				new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).Field(nameof(Game1.random))),
				new CodeInstruction(OpCodes.Callvirt, typeof(Random).MethodNamed(nameof(Random.NextDouble)))
			);
		}

		/// <summary>Insert a sequence of code instructions at the currently pointed index to roll a random integer.</summary>
		/// <param name="minValue">The lower limit, inclusive.</param>
		/// <param name="maxValue">The upper limit, inclusive.</param>
		public ILHelper InsertDiceRoll(int minValue, int maxValue)
		{
			return Insert(
				new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).Field(nameof(Game1.random))),
				LoadConstantIntegerIL(minValue),
				LoadConstantIntegerIL(maxValue + 1),
				new CodeInstruction(OpCodes.Callvirt, typeof(Random).MethodNamed(nameof(Random.Next)))
			);
		}

		/// <summary>Remove code instructions starting from the currently pointed index.</summary>
		/// <param name="count">Number of code instructions to remove.</param>
		public ILHelper Remove(int count = 1)
		{
			if (CurrentIndex + count > LastIndex) throw new IndexOutOfRangeException("Can't remove item out of range.");

			_instructionList.RemoveRange(CurrentIndex, count);
			return this;
		}

		/// <summary>Remove code instructions starting from the currently pointed index until a specific pattern is found.</summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper RemoveUntil(params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - CurrentIndex;
			_instructionList.RemoveRange(CurrentIndex, count);

			return this;
		}

		/// <summary>Remove code instructions starting from the currently pointed index until a specific label is found.</summary>
		/// <param name="label">The <see cref="Label" /> object to match.</param>
		public ILHelper RemoveUntilLabel(Label label)
		{
			AdvanceUntilLabel(label);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - CurrentIndex;
			_instructionList.RemoveRange(CurrentIndex, count);

			return this;
		}

		/// <summary>Copy code instructions starting from the currently pointed index to the buffer.</summary>
		/// <param name="count">Number of code instructions to copy.</param>
		/// <param name="stripLabels">Whether to remove the labels from the copied instructions.</param>
		/// <param name="advance">Whether to advance the index pointer.</param>
		public ILHelper ToBuffer(int count = 1, bool stripLabels = false, bool advance = false)
		{
			_instructionBuffer = _instructionList.GetRange(CurrentIndex, count).Clone();

			if (stripLabels) StripBufferLabels();

			if (advance) _indexStack.Push(CurrentIndex + count);

			return this;
		}

		/// <summary>
		///     Copy code instructions starting from the currently pointed index until a specific pattern is found to the
		///     buffer.
		/// </summary>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper ToBufferUntil(params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - CurrentIndex;
			_instructionBuffer = _instructionList.GetRange(CurrentIndex, count).Clone();

			return this;
		}

		/// <summary>
		///     Copy code instructions starting from the currently pointed index until a specific pattern is found to the
		///     buffer.
		/// </summary>
		/// <param name="stripLabels">Whether to remove the labels from the copied instructions.</param>
		/// <param name="advance">Whether to advance the index pointer.</param>
		/// <param name="pattern">Sequence of <see cref="CodeInstruction" /> objects to match.</param>
		public ILHelper ToBufferUntil(bool stripLabels, bool advance, params CodeInstruction[] pattern)
		{
			AdvanceUntil(pattern);

			var endIndex = _indexStack.Pop() + 1;
			var count = endIndex - CurrentIndex;
			_instructionBuffer = _instructionList.GetRange(CurrentIndex, count).Clone();

			if (stripLabels) StripBufferLabels();

			if (advance) _indexStack.Push(endIndex);

			return this;
		}

		/// <summary>Get the labels from the code instruction at the currently pointed index.</summary>
		/// <param name="labels">The returned list of <see cref="Label" /> objects.</param>
		public ILHelper GetLabels(out List<Label> labels)
		{
			labels = _instructionList[CurrentIndex].labels.ToList();
			return this;
		}

		/// <summary>Add one or more labels to the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A sequence of <see cref="Label" /> objects to add.</param>
		public ILHelper AddLabels(params Label[] labels)
		{
			_instructionList[CurrentIndex].labels.AddRange(labels);
			return this;
		}

		/// <summary>Add one or more labels to the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A sequence of <see cref="Label" /> objects to add.</param>
		public ILHelper AddLabels(ICollection<Label> labels)
		{
			_instructionList[CurrentIndex].labels.AddRange(labels);
			return this;
		}

		/// <summary>Set the labels of the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A list of <see cref="Label" /> objects.</param>
		public ILHelper SetLabels(params Label[] labels)
		{
			_instructionList[CurrentIndex].labels = labels.ToList();
			return this;
		}

		/// <summary>Set the labels of the code instruction at the currently pointed index.</summary>
		/// <param name="labels">A list of <see cref="Label" /> objects.</param>
		public ILHelper SetLabels(ICollection<Label> labels)
		{
			_instructionList[CurrentIndex].labels = labels.ToList();
			return this;
		}

		/// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
		public ILHelper StripLabels()
		{
			_instructionList[CurrentIndex].labels.Clear();
			return this;
		}

		/// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
		public ILHelper StripLabels(out List<Label> labels)
		{
			GetLabels(out labels);
			_instructionList[CurrentIndex].labels.Clear();
			return this;
		}

		/// <summary>Remove any labels from code instructions currently in the buffer.</summary>
		private void StripBufferLabels()
		{
			foreach (var instruction in _instructionBuffer) instruction.labels.Clear();
		}

		/// <summary>Return the opcode of the code instruction at the currently pointed index.</summary>
		/// <param name="opcode">The returned <see cref="OpCode" /> object.</param>
		public ILHelper GetOpCode(out OpCode opcode)
		{
			opcode = _instructionList[CurrentIndex].opcode;
			return this;
		}

		/// <summary>Change the opcode of the code instruction at the currently pointed index.</summary>
		/// <param name="opcode">The new <see cref="OpCode" /> object.</param>
		public ILHelper SetOpCode(OpCode opcode)
		{
			_instructionList[CurrentIndex].opcode = opcode;
			return this;
		}

		/// <summary>Return the operand of the code instruction at the currently pointed index.</summary>
		/// <param name="operand">The returned operand <see cref="object" />.</param>
		public ILHelper GetOperand(out object operand)
		{
			operand = _instructionList[CurrentIndex].operand;
			return this;
		}

		/// <summary>Change the operand of the code instruction at the currently pointed index.</summary>
		/// <param name="operand">The new <see cref="object" /> operand.</param>
		public ILHelper SetOperand(object operand)
		{
			_instructionList[CurrentIndex].operand = operand;
			return this;
		}

		/// <summary>Reset the current instance.</summary>
		public ILHelper Clear()
		{
			_indexStack.Clear();
			_instructionList.Clear();
			return this;
		}

		/// <summary>Reset the instance and return the active code instruction list as enumerable.</summary>
		public IEnumerable<CodeInstruction> Flush()
		{
			var result = _instructionList.Clone();
			Clear();
			return result.AsEnumerable();
		}

		/// <summary>Export the failed search target and active code instruction list to a text file.</summary>
		public void Export(List<CodeInstruction> pattern)
		{
			if (!Directory.Exists(_exportDir)) Directory.CreateDirectory(_exportDir);

			var path = Path.Combine(_exportDir,
				($"{_original.DeclaringType}.{_original.Name}".Replace('.', '_') + ".cil").RemoveInvalidChars());
			using var writer = File.CreateText(path);
			writer.WriteLine("Searching for:");
			pattern.ForEach(l => writer.WriteLine(l.ToString()));
			writer.WriteLine("\n <-- START OF INSTRUCTION LIST -->\n");
			_instructionList.ForEach(l => writer.WriteLine(l.ToString()));
			writer.WriteLine("\n<-- END OF INSTRUCTION LIST -->");
		}

		/// <summary>Export the failed search target and active code instruction list to a text file.</summary>
		public void Export(Label label)
		{
			if (!Directory.Exists(_exportDir)) Directory.CreateDirectory(_exportDir);

			var path = Path.Combine(_exportDir,
				($"{_original.DeclaringType}.{_original.Name}".Replace('.', '_') + ".cil").RemoveInvalidChars());
			using var writer = File.CreateText(path);
			writer.WriteLine("Searching for:\n");
			writer.WriteLine(label.ToString());
			writer.WriteLine("\n <-- START OF INSTRUCTION LIST -->\n");
			_instructionList.ForEach(l => writer.WriteLine(l.ToString()));
			writer.WriteLine("\n<-- END OF INSTRUCTION LIST -->");
		}

		/// <summary>Get the corresponding IL code instruction which loads a given integer.</summary>
		/// <param name="number">An integer.</param>
		private static CodeInstruction LoadConstantIntegerIL(int number)
		{
			return number switch
			{
				0 => new(OpCodes.Ldc_I4_0),
				1 => new(OpCodes.Ldc_I4_1),
				2 => new(OpCodes.Ldc_I4_2),
				3 => new(OpCodes.Ldc_I4_3),
				4 => new(OpCodes.Ldc_I4_4),
				5 => new(OpCodes.Ldc_I4_5),
				6 => new(OpCodes.Ldc_I4_6),
				7 => new(OpCodes.Ldc_I4_7),
				8 => new(OpCodes.Ldc_I4_8),
				_ => new(OpCodes.Ldc_I4_S, number)
			};
		}
	}
}