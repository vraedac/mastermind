using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind
{
	class Program
	{
		const string ErrString = "Input must be a string of four digits from 1 to 6, please try again.";

		static void Main(string[] args)
		{
			int[] solution = GenerateSolution(); 
			var game = new Mastermind(solution);

			while(!game.IsComplete)
			{
				var input = Console.ReadLine();

				try
				{
					var (result, hint) = game.Guess(input);

					if (result)
						Console.WriteLine("Congratulations, you've won!");
					else if (!game.IsComplete)
						Console.WriteLine($"Hint: {hint}; {game.GuessesRemaining} guesses remaining.");
					else
						Console.WriteLine("Sorry, you have lost!");

					if (game.IsComplete)
					{
						Console.WriteLine("Press any key to exit.");
						Console.ReadKey();
						break;
					}
				}
				catch
				{
					Console.Error.WriteLine(ErrString);
				}
			}

		}

		static int[] GenerateSolution()
		{
			var random = new Random();
			var result = new List<int>();

			for (var i = 0; i < 4; i++)
				result.Add(random.Next(1, 7));

			return result.ToArray();
		}
	}
}
