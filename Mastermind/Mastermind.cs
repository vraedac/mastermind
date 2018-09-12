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
				{
					correct++;
					checkedNumbers.Add(guess[i]);
				}
			}

			for (int i = 0; i < guess.Length; i++)
			{
				if (_solution.Contains(guess[i]) && !checkedNumbers.Contains(guess[i]))
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
			if (input.Length != 4)
				throw new ArgumentException(ErrString);

			bool inputCheck = int.TryParse(input[0].ToString(), out var thousands);
			inputCheck &= int.TryParse(input[1].ToString(), out var hundreds);
			inputCheck &= int.TryParse(input[2].ToString(), out var tens);
			inputCheck &= int.TryParse(input[3].ToString(), out var ones);

			if (!inputCheck || thousands > 6 || hundreds > 6 || tens > 6 || ones > 6)
				throw new ArgumentException(ErrString);

			return new int[] { thousands, hundreds, tens, ones };
		}
	}
}
