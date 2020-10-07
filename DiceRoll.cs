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
	static class DiceRoll
	{
		public static async Task Execute(SocketUserMessage msg)
		{
			ProcessTaarget["+"] = ProcessTaargetAdd;
			ProcessTaarget["-"] = ProcessTaargetSub;
			ProcessTaarget["*"] = ProcessTaargetMul;
			ProcessTaarget["/"] = ProcessTaargetDiv;

			if (Regex.IsMatch(msg.ToString(), @"(?i)^&help$"))
			{
				await msg.Channel.SendMessageAsync(
					"実は本当のプレフィックスは `%` です。\n" +
					"コマンドは現在、`%help` `%version(ver)` `%git` があります。"
					);

				return;
			}

			string send_msg = msg.Author.Mention + '\n';

			string dice_area = msg.Content.Split(new char[] { ':', ';' }).Last().Replace(" ", "").Replace("　", "");

			Match per_check = Regex.Match(dice_area, @"(?i)^&\s*(percent|per|p)");
			Match tar = Regex.Match(dice_area, @"(?i)(?<=^&\s*(percent|per|p)\s*)\d+");

			int finaly_tar = 0;

			if (per_check.Length != 0) {
				MatchCollection tar_fixs = Regex.Matches(dice_area, @"(?i)(?<=\d+\s*)(?<ope>(\+|\-|\*|/))\s*(?<val>\d+)");
				dice_area = "1d100 tar=";
				if (tar.Length != 0)
				{
					finaly_tar = int.Parse(tar.Value);
					foreach (Match m in tar_fixs)
					{
						ProcessTaarget[m.Groups["ope"].Value](ref finaly_tar, int.Parse(m.Groups["val"].Value));
					}

					dice_area += finaly_tar.ToString();
				}
			}

				ISocketMessageChannel channel = msg.Channel;

			RandomManager.ClearHistory();

			MatchCollection dices = Regex.Matches(dice_area, @"(?i)(?<sign>(\+|\-|))(?<value>\d+)(?<type>(d|r))(?<sides>\d+)((\[|@)(?<critical>\d+)(\]|))?");
			MatchCollection fixes = Regex.Matches(dice_area, @"(?i)(?<fix>(\+|\-)\d+)(?=(\+|\-|$))");
			MatchCollection dxs = Regex.Matches(dice_area, @"(?i)(?<sign>(\+|\-|))(?<value>\d+)dx((\[|@)(?<critical>\d+)(\]|))?");
			Match target_match = Regex.Match(dice_area, @"(?i)(target|tar|trg|tgt)=\d+");

			if (target_match.Success)
			{
				send_msg += "Target -> " + finaly_tar + "\n";
			}

			int d, r;
			d = r = 0;

			foreach (Match m in dices)
			{
				string res = "";

				if (m.Groups["type"].Value.ToLower() == "d")
				{
					res += NDXAsync(channel, int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), m.Groups["sign"].ToString() == "-").Result;
					d++;
				}
				else if (m.Groups["type"].Value.ToLower() == "r")
				{
					if (m.Groups["critical"].Value == "")
					{
						res += NRXAsync(channel, int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value)).Result;
					}
					else
					{
						res += NRXAsync(channel, int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), int.Parse(m.Groups["critical"].Value)).Result;
					}
					r++;
				}

				if (res.EndsWith("failed"))
				{
					return;
				}
				else
				{
					send_msg += res + '\n';
				}
			}

			foreach (Match m in dxs)
			{
				string res = "";

				if (m.Groups["critical"].Value == "")
				{
					res += NRXAsync(channel, int.Parse(m.Groups["value"].Value), 10).Result;
				}
				else
				{
					res += NRXAsync(channel, int.Parse(m.Groups["value"].Value), 10, int.Parse(m.Groups["critical"].Value)).Result;
				}

				if (res.EndsWith("failed"))
				{
					return;
				}
				else
				{
					send_msg += res + '\n';
				}
				r++;
			}

			if(d + r == 0)
			{
				return;
			}

			int fix_sum = 0;

			foreach (Match m in fixes)
			{
				fix_sum += int.Parse(m.Groups["fix"].Value);
			}

			send_msg += "\nResult: " + RandomManager.DiceResultHistory.Sum();

			if (fixes.Count != 0)
			{
				send_msg += ' ' + fix_sum.ToString("+ #;- #") + " -> " + (RandomManager.DiceResultHistory.Sum() + fix_sum);
			}

			if(target_match.Success)
			{
				string target = target_match.Value.Split('=')[1];

				if (5 >= (RandomManager.DiceResultHistory.Sum() + fix_sum))
				{
					send_msg += "    _**__Critical__**_";
				}
				else if(int.Parse(target) >= (RandomManager.DiceResultHistory.Sum() + fix_sum))
				{
					send_msg += "    **Success**";
				}
				else if(95 >= (RandomManager.DiceResultHistory.Sum() + fix_sum))
				{
					send_msg += "    **Fail**";
				}
				else
				{
					send_msg += "    _**__Famble__**_";
				}
			}

			if(send_msg.Length > 2000)
			{
				send_msg = send_msg.Replace(msg.Author.Mention, "\n" + msg.Author.Username);
				Console.WriteLine(send_msg);

				send_msg = msg.Author.Mention + '\n';
				send_msg += "文字数が多くなっちゃったから、結果だけ伝えるね。\n(詳細はコンソール画面を見てね)\n";

				send_msg += "\nResult: " + RandomManager.DiceResultHistory.Sum();

				if (fixes.Count != 0)
				{
					send_msg += ' ' + fix_sum.ToString("+ #;- #") + " -> " + (RandomManager.DiceResultHistory.Sum() + fix_sum);
				}
			}

			await msg.Channel.SendMessageAsync(send_msg);

		}

		private static async Task<string> NDXAsync(ISocketMessageChannel channel, int value, int sides, bool Negative = false)
		{
			// 制約チェック
			if (!await RengeCheckAsync(channel, value, sides))
			{
				return "failed";
			}

			// 戻り文字列初期化
			string ret_str = "";

			// 戻り文字列生成
			List<int> res = new List<int>();
			for (int i = 0; i < value; i++)
			{
				res.Add(RandomManager.Rand(sides, true, Negative));
			}

			ret_str += value + "D" + sides + " -> ";

			ret_str += GenerateDiceRollString(res, res.Count);

			ret_str += " -> " + res.Sum();

			return ret_str;
		}

		private static async Task<string> NRXAsync(ISocketMessageChannel channel, int value, int sides, int critical = 10, bool Negative = false)
		{
			critical = Math.Max(Math.Min(critical, sides), 2);

			// 制約チェック
			if (!await RengeCheckAsync(channel, value, sides))
			{
				return "failed";
			}

			string ret_str = "";

			// 一回目
			List<int> res = new List<int>();
			for (int i = 0; i < value; i++)
			{
				res.Add(RandomManager.Rand(sides, false));
			}

			int critical_value = res.Count(n => n >= critical);
			int ctimes = 0;
			int last_max = 0;
			res.Sort();

			ret_str += value + "DX" + (critical != 10 ? ("@" + critical) : "") + " -> ";

			// 2回目以降
			while (res.Count > 0)
			{
				if (critical_value > 0)
				{
					ctimes++;
				}

				ret_str += GenerateDiceRollString(res, res.Count, isCritical: true, critical: critical);

				last_max = res.Max();

				res.Clear();

				for (int i = 0; i < critical_value; i++)
				{
					res.Add(RandomManager.Rand(sides, false));
				}

				critical_value = res.Count(n => n >= critical);
				res.Sort();

				ret_str += " -> ";
			}

			int dice_res = ctimes * sides + last_max;

			ret_str += dice_res;

			RandomManager.DiceResultHistory.Add(dice_res);

			return ret_str;
		}

		private static async Task<bool> RengeCheckAsync(ISocketMessageChannel channel, int value, int sides)
		{
			if (value > 100)
			{
				await channel.SendMessageAsync("100 個を超えるダイスは用意できないかな...");
				return false;
			}
			else if (sides > 99999)
			{
				await channel.SendMessageAsync("99999 面よりも面の数が多いダイスは用意できないね...");
				return false;
			}

			return true;
		}

		private static string GenerateDiceRollString(List<int> diceRes, int count, bool isCritical = false, int critical = 0)
		{
			string ret_str = "[";

			for (int i = 0; i < count; i++)
			{
				if (isCritical && diceRes[i] >= critical)
				{
					ret_str += "**" + diceRes[i] + "**";
				}
				else
				{
					ret_str += diceRes[i];
				}

				if (i < count - 1)
				{
					ret_str += ", ";
				}
			}

			ret_str += "]";

			return ret_str;
		}

		private delegate void ProcessTargetDelegate(ref int tar, int val);

		private static Dictionary<string, ProcessTargetDelegate> ProcessTaarget = new Dictionary<string, ProcessTargetDelegate>();

		static private void ProcessTaargetAdd(ref int tar, int val)
		{
			tar += val;
		}

		static private void ProcessTaargetSub(ref int tar, int val)
		{
			tar -= val;
		}

		static private void ProcessTaargetMul(ref int tar, int val)
		{
			tar *= val;
		}

		static private void ProcessTaargetDiv(ref int tar, int val)
		{
			tar /= val;
		}
	}
}
