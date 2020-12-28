using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace TrpgDiceBot
{
	static class CharacterCreateCoC
	{
		private static Dictionary<string, Tuple<int, int>> _prmInfo = new Dictionary<string, Tuple<int, int>>();
		private static Dictionary<string, int> _prmResult = new Dictionary<string, int>();

		public static async Task Create(SocketUserMessage msg)
		{
			StorePrmInfos();

			string send_msg = msg.Author.Mention + "\nType : CoC\n";

			foreach (var v in _prmInfo)
			{
				for (int i = 0; i < v.Value.Item1; i++)
				{
					RandomManager.Rand(6);
				}

				_prmResult[v.Key] = RandomManager.DiceResultHistory.Sum() + v.Value.Item2;
				send_msg += v.Key + " : " + _prmResult[v.Key] + " ([";

				for (int i = 0; i < v.Value.Item1; i++)
				{
					send_msg += RandomManager.DiceResultHistory[i];
					if (i != v.Value.Item1 - 1)
					{
						send_msg += ", ";
					}
					else
					{
						send_msg += "]";
					}
				}

				if(v.Value.Item2 != 0)
				{
					send_msg += " + " + v.Value.Item2;
				}

				send_msg += ")\n";

				RandomManager.ClearHistory();
			}

			send_msg += "\n";

			send_msg += "HP : " + (_prmResult["CON"] + _prmResult["SIZ"]) / 2 + " ((CON + SIZ) / 2)\n";
			send_msg += "MP : " + _prmResult["POW"] + " (POW)\n";
			send_msg += "SAN : " + _prmResult["CON"] * 5 + " (CON * 5)\n";
			send_msg += "ダメボ : " + CalcDamageBonus(_prmResult["STR"] + _prmResult["SIZ"]) + "\n";
			send_msg += "アイデア : " + _prmResult["INT"] * 5 + " (INT * 5)\n";
			send_msg += "知識 : " + _prmResult["EDU"] * 5 + " (EDU * 5)\n";
			send_msg += "幸運 : " + _prmResult["POW"] * 5 + " (POW * 5)";

			await msg.Channel.SendMessageAsync(send_msg);
		}

		private static void StorePrmInfos()
		{
			_prmInfo["STR"] = new Tuple<int, int>(3, 0);
			_prmInfo["CON"] = new Tuple<int, int>(3, 0);
			_prmInfo["POW"] = new Tuple<int, int>(3, 0);
			_prmInfo["DEX"] = new Tuple<int, int>(3, 0);
			_prmInfo["APP"] = new Tuple<int, int>(3, 0);
			_prmInfo["SIZ"] = new Tuple<int, int>(2, 6);
			_prmInfo["INT"] = new Tuple<int, int>(2, 6);
			_prmInfo["EDU"] = new Tuple<int, int>(3, 3);
		}

		private static string CalcDamageBonus(int val)
		{
			if(val >= 2 && val <= 12)
			{
				return "-1D6";
			}
			if (val >= 13 && val <= 16)
			{
				return "-1D4";
			}
			if (val >= 17 && val <= 24)
			{
				return "+0";
			}
			if (val >= 25 && val <= 32)
			{
				return "+1D4";
			}
			else
			{
				return "+1d6";
			}
		}
	}
}
