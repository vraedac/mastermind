using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Mastermind;

namespace Mastermind.Tests
{
	[TestFixture]
	public class MastermindTests
	{
		// i don't think hint format is completely correct

		#region Validation

		[Test]
		public void Ctor_SolutionContainsInvalidNumber_Throws() => this.Invoking(x => new Mastermind(new int[] { 1, 3, 5, 9 })).Should().Throw<ArgumentException>();

		[Test]
		public void Ctor_SolutionContainsTooManyNumbers_Throws() => this.Invoking(x => new Mastermind(new int[] { 1, 2, 3, 4, 5 })).Should().Throw<ArgumentException>();

		[Test]
		public void Ctor_SolutionContainsTooFewNumbers_Throws() => this.Invoking(x => new Mastermind(new int[] { 1, 2, 3 })).Should().Throw<ArgumentException>();

		[Test]
		public void Guess_InputNonNumeric_Throws()
		{
			// Arrange
			string guess1 = "some string",
				guess2 = "124d",
				guess3 = " a22";

			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });

			// Act/Assert
			underTest.Invoking(x => x.Guess(guess1)).Should().Throw<ArgumentException>();
			underTest.Invoking(x => x.Guess(guess2)).Should().Throw<ArgumentException>();
			underTest.Invoking(x => x.Guess(guess3)).Should().Throw<ArgumentException>();
		}

		[Test]
		public void Guess_InputContainsInvalidNumber_Throws()
		{
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });
			underTest.Invoking(x => x.Guess("1238")).Should().Throw<ArgumentException>();
		}

		[Test]
		public void Guess_InputContainsTooManyNumbers_Throws()
		{
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });
			underTest.Invoking(x => x.Guess("12345")).Should().Throw<ArgumentException>();
		}

		[Test]
		public void Guess_InputContainsTooFewNumbers_Throws()
		{
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });
			underTest.Invoking(x => x.Guess("123")).Should().Throw<ArgumentException>();
		}

		#endregion Validation

		#region Behavior

		[Test]
		public void MatchesGuessCorrectly([Range(1, 6)]int first, [Range(1, 6)]int second, [Range(1, 6)]int third, [Range(1, 6)]int fourth)
		{
			// Arrange
			var underTest = new Mastermind(new int[] { first, second, third, fourth });

			// Act
			var result = underTest.Guess($"{first}{second}{third}{fourth}");

			// Assert
			result.Result.Should().BeTrue();
		}

		[Test]
		public void Guess_DecrementsGuessesRemaining()
		{
			// Arrange
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });

			// Act
			underTest.Guess("1231");

			// Assert
			underTest.GuessesRemaining.Should().Be(9);
		}

		[Test]
		public void Guess_WithCorrectSolution_EndsGame()
		{
			// Arrange
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });

			// Act
			underTest.Guess("1234");

			// Assert
			underTest.IsComplete.Should().BeTrue();
		}

		[Test]
		public void Guess_DoesNotEndGameUntilTenthIncorrectAttempt()
		{
			// Arrange
			Mastermind underTest1 = new Mastermind(new int[] { 1, 2, 3, 4 }),
				underTest2 = new Mastermind(new int[] { 2, 3, 4, 5 });

			for (int i = 0; i < 9; i++)
				underTest1.Guess("1111");

			// Act
			for (int i = 0; i < 9; i++)
				underTest2.Guess("1111");

			underTest1.Guess("1111");

			// Assert
			underTest1.IsComplete.Should().BeTrue();
			underTest2.IsComplete.Should().BeFalse();
		}

		[Test]
		public void Guess_ThrowsIfAttemptedAfterGameWon()
		{
			// Arrange
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });
			underTest.Guess("1234");

			// Act/Assert
			underTest.Invoking(x => x.Guess("1234")).Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Guess_ThrowsIfAttemptedAfterGameLost()
		{
			// Arrange
			var underTest = new Mastermind(new int[] { 1, 2, 3, 4 });

			for (int i = 0; i < 10; i++)
				underTest.Guess("1111");

			// Act/Assert
			underTest.Invoking(x => x.Guess("1111")).Should().Throw<InvalidOperationException>();
		}

		#endregion Behavior


		#region Output

		private void HintTestBody(int[] solution, string guess, string expectedHint, bool expectedResult = false)
		{
			// Arrange
			var underTest = new Mastermind(solution);

			// Act
			var (result, hint) = underTest.Guess(guess);

			// Assert
			result.Should().Be(expectedResult);
			hint.Should().Be(expectedHint);
		}

		[Test]
		public void Guess_OneCorrectNumberInCorrectLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "1556", "+");

		[Test]
		public void Guess_TwoCorrectNumbersInCorrectLocation_AtLeft_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "1256", "++");

		[Test]
		public void Guess_TwoCorrectNumbersInCorrectLocation_AtRight_ReturnsCorrectResult() => HintTestBody(new int[] { 2, 5, 1, 1 }, "1111", "++");

		[Test]
		public void Guess_ThreeCorrectNumbersInCorrectLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "1236", "+++");

		[Test]
		public void Guess_CorrectSolution_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "1234", "++++", expectedResult: true);

		[Test]
		public void Guess_OneCorrectNumberInWrongLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "5516", "-");

		[Test]
		public void Guess_RepeatedNumberInWrongLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "5111", "-");

		[Test]
		public void Guess_RepeatedNumber_OneLocationIsCorrect_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 3, 2, 6 }, "1111", "+");

		[Test]
		public void Guess_TwoCorrectNumbersInWrongLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "5512", "--");

		[Test]
		public void Guess_ThreeCorrectNumbersInWrongLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "5312", "---");
		
		[Test]
		public void Guess_FourCorrectNumbersInWrongLocation_ReturnsCorrectResult() => HintTestBody(new int[] { 1, 2, 3, 4 }, "4312", "----");

		[Test]
		public void Guess_SolutionContainsRepeatingDigit_GuessContainsItInWrongLocation() => HintTestBody(new int[] { 1, 1, 2, 3 }, "4415", "-");


		#endregion Output

	}
}
