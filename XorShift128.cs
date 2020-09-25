using System;
using System.Collections.Generic;
using System.Text;

namespace TrpgDiceBot
{
	static class XorShift
	{
		private struct RandDic
		{
			uint x;
			uint y;
			uint z;
			uint w;

			public RandDic(uint inX, uint inY, uint inZ, uint inW)
			{
				x = inX;
				y = inY;
				z = inZ;
				w = inW;
			}
		}

		private static uint _x, _y, _z, _w, _t;

		public static uint _randCount = 0;
		private static RandDic _seeds;

		public static void SetSeed()
		{
			_w = (uint)(Environment.TickCount ^ ((Environment.TickCount >> 24) & 0xff) ^ (Environment.TickCount << 26));
			_x = _w << 13;
			_y = (_w >> 9) ^ (_x << 6);
			_z = _y >> 7;

			_seeds = new RandDic(_x, _y, _z, _w);
		}

		public static void SetSeed(uint w)
		{
			_w = w;
			_x = _w << 13;
			_y = (_w >> 9) ^ (_x << 6);
			_z = _y >> 7;

			_seeds = new RandDic(_x, _y, _z, _w);
		}

		public static uint Rand()
		{
			++_randCount;
			uint t = _x ^ (_x << 11);
			_x = _y;
			_y = _z;
			_z = _w;
			return _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
		}

		public static int RandInt(int min = 0, int max = 0x7fffffff)
		{
			return (int)(Rand() % (max - min + 1) + min);
		}

		public static float RandFloat(float min = 0, float max = 1)
		{
			return (float)(Rand() % 0xffff) / 0xffff * (max - min) + min;
		}
	}
}
