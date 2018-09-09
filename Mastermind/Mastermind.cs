using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	/// <summary>
	/// Represents a single game of Mastermind.  Allows 10 guesses before a new instance must be created (thus starting a new game).
	/// </summary>
	public class Mastermind
	{
		public int GuessesRemaining { get; private set; }

		public bool IsComplete { get; private set; }

		private const string ErrString = "Guess must be a string of four digits from 1 to 6, please try again.";

		private readonly int[] _solution;

		public Mastermind(int[] solution)
		{
			validateSolution();
			_solution = solution;
			GuessesRemaining = 10;

			void validateSolution()
			{
				var message = "The solution must be exactly four digits between 1 and 6.";

				if (solution.Length != 4)
					throw new ArgumentException(message);

				foreach (var num in solution)
				{
					if (num < 1 || num > 6)
						throw new ArgumentException(message);
				}
			}
		}

		/// <summary>
		/// Checks the player's guess against the internally generated solution.
		/// </summary>
		/// <param name="input">The input string entered by the player</param>
		/// <returns>A { bool, string } touple indicating whether the player's guess was correct, and providing the hint string</returns>
		public (bool Result, string Hint) Guess(string input)
		{
			if (IsComplete)
				throw new InvalidOperationException("This Mastermind game has been completed, instantiate a new one to continue playing.");

			int[] guess = ConvertInputToGuessArray(input);

			int correct = 0, partialCorrect = 0;
			string hint = string.Empty;
			bool result = false;
			var checkedNumbers = new List<int>();

			for (int i = 0; i < guess.Length; i++)
			{
				if (guess[i] == _solution[i])
					correct++;
				else if (_solution.Contains(guess[i]) && !checkedNumbers.Contains(guess[i]))
					partialCorrect++;

				checkedNumbers.Add(guess[i]);
			}

			result = correct == 4;

			while (correct-- > 0)
				hint += '+';
			while (partialCorrect-- > 0)
				hint += '-';

			if (result || --GuessesRemaining == 0)
				IsComplete = true;

			return (result, hint);
		}

		/// <summary>
		/// Validates the player's input and converts it into an int array representing their guess.  Throws an exception if validation fails.
		/// </summary>
		/// <param name="input">The input string entered by the player</param>
		/// <returns>The player's guess in the form of an int[4]</returns>
		private int[] ConvertInputToGuessArray(string input)
		{
			bool inputCheck = int.TryParse(input, out var guess);

			if (!inputCheck || input.Length != 4)
				throw new ArgumentException(ErrString);

			return checkDigits(guess);

			int[] checkDigits(int value)
			{
				// 7152
				if (value - 6000 >= 1000)
					throw new ArgumentException(ErrString);

				// 2751
				var thx = value / 1000; // 2
				var hundreds = value - (1000 * thx); // 2751 - (1000 * 2) = 751
				if (hundreds - 600 >= 100) throw new ArgumentException(ErrString); // 751 - 600 = 151

				// 2271
				var hx = hundreds / 100; // 2
				var tens = hundreds - (100 * hx); // 271 - (100 * 2) = 71
				if (tens - 60 >= 10) throw new ArgumentException(ErrString);  // 71 - 60 = 11

				// 2457
				var tex = tens / 10; // 5
				var ones = tens - (10 * tex); // 57 - (10 * 5) = 7
				if (ones - 6 >= 1) throw new ArgumentException(ErrString); // 7 - 6 = 1

				return new int[] { thx, hx, tex, ones };
			}
		}
	}
}
