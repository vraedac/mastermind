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

			while(true)
			{
				var input = Console.ReadLine();

				if (!CheckInput(input))
					Console.Error.WriteLine(ErrString);
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

		static bool CheckInput(string input)
		{
			bool inputCheck = int.TryParse(input, out var guess);

			if (!inputCheck)
				return false;

			if (input.Length > 4)
				return false;

			return checkDigits(guess);

			bool checkDigits(int value)
			{
				// 7152
				if (value - 6000 >= 1000)
					return false;

				// 2751
				var thx = value / 1000; // 2
				var hundreds = value - (1000 * thx); // 2751 - (1000 * 2) = 751
				if (hundreds - 600 >= 100) return false; // 751 - 600 = 151

				// 2271
				var hx = hundreds / 100; // 2
				var tens = hundreds - (100 * hx); // 271 - (100 * 2) = 71
				if (tens - 60 >= 10) return false; // 71 - 60 = 11

				// 2457
				var tex = tens / 10; // 5
				var ones = tens - (10 * tex); // 57 - (10 * 5) = 7
				if (ones - 6 >= 1) return false; // 7 - 6 = 1

				return true;
			}
		}
	}
}
