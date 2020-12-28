using System;
using System.Collections.Generic;
using System.Text;

namespace TrpgDiceBot
{
	static class RandomManager
	{
		private static bool _setSeed = false;
		private static DateTime _setSeedTime;

		public static List<int> DiceResultHistory { get; set; } = new List<int>();

		public static int Rand(int sided, bool addList = true, bool Negative = false)
		{
			if (!_setSeed || (DateTime.Now - _setSeedTime).Minutes > 3) {
				_setSeed = true;
				_setSeedTime = DateTime.Now;
				XorShift.SetSeed();

				for (int i = 0; i < XorShift.RandInt(50, 100); i++)
				{
					XorShift.Rand();
				}
			}

			int ret;

			ret =  XorShift.RandInt(1, sided);

			if (addList)
			{
				DiceResultHistory.Add(ret * (Negative ? -1 : 1));
			}

			return ret;
		}

		public static void ClearHistory()
		{
			DiceResultHistory.Clear();
		}
	}
}
