using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using System.Linq;

namespace TrpgDiceBot
{
	[Group("d")]
	public class DiceModule : ModuleBase<SocketCommandContext>
	{
		[Command("od")]
		public async Task DiceRollAsync(string diceFormat, int target = 0)
		{
			if (Regex.IsMatch(diceFormat, @"^(?i)(.+(:|;)|)(?<value>\d+)d(?<sides>\d+)(?<fix>(\+|\-)\d)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(.+(:|;)|)(?<value>\d+)d(?<sides>\d+)(?<fix>(\+|\-)\d)$");
				await NDXAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), fix: int.Parse(m.Groups["fix"].Value), target: target);
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(.+(:|;)|)(?<value>\d+)d(?<sides>\d+)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(.+(:|;)|)(?<value>\d+)d(?<sides>\d+)$");
				await NDXAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), target: target);
			}

			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)(@(?<critical>\d+)|\[(?<critical>\d+)\])(?<fix>(\+|\-)\d+)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)(@(?<critical>\d+)|\[(?<critical>\d+)\])(?<fix>(\+|\-)\d+)$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), critical: int.Parse(m.Groups["critical"].Value), fix: int.Parse(m.Groups["fix"].Value));
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)(?<fix>(\+|\-)\d+)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)(?<fix>(\+|\-)\d+)$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), fix: int.Parse(m.Groups["fix"].Value));
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)(@(?<critical>\d+)|\[(?<critical>\d+)\])$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)(@(?<critical>\d+)|\[(?<critical>\d+)\])$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), critical: int.Parse(m.Groups["critical"].Value));
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)r(?<sides>\d+)$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value));
			}

			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)dx(@(?<critical>\d+)|\[(?<critical>\d+)\])(?<fix>(\+|\-)\d+)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)dx(@(?<critical>\d+)|\[(?<critical>\d+)\])(?<fix>(\+|\-)\d+)$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), 10, critical: int.Parse(m.Groups["critical"].Value), fix: int.Parse(m.Groups["fix"].Value));
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)dx(?<fix>(\+|\-)\d+)$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)dx(?<fix>(\+|\-)\d+)$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), 10, fix: int.Parse(m.Groups["fix"].Value));
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)dx(@(?<critical>\d+)|\[(?<critical>\d+)\])$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)dx(@(?<critical>\d+)|\[(?<critical>\d+)\])$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), 10, critical: int.Parse(m.Groups["critical"].Value));
			}
			else if (Regex.IsMatch(diceFormat, @"^(?i)(?<value>\d+)dx$"))
			{
				var m = Regex.Match(diceFormat, @"^(?i)(?<value>\d+)dx$");
				await NRXAsync(int.Parse(m.Groups["value"].Value), 10);
			}

			else
			{
				await Context.Channel.SendMessageAsync("no match");
			}
		}

		[Command("ft")]
		public async Task MixFormatAsync(string format)
		{
			string send_msg = Context.User.Mention + '\n';

			string dice_area = format.Split(new char[] { ':', ';' }).Last();

			RandomManager.ClearHistory();

			MatchCollection dices = Regex.Matches(dice_area, @"(?i)(\+|\-|)(?<value>\d+)(?<type>(d|r))(?<sides>\d+)((\[|@)(?<critical>\d+)(\]|))?");
			MatchCollection fixes = Regex.Matches(dice_area, @"(?i)(?<fix>(\+|\-)\d+)(?![a-z])");
			MatchCollection dxs = Regex.Matches(dice_area, @"(?i)(\+|\-|)(?<value>\d+)dx((\[|@)(?<critical>\d+)(\]|))?");

			foreach (Match m in dices)
			{
				string res = "";

				if (m.Groups["type"].Value.ToLower() == "d")
				{
					res += NDXFTAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value)).Result;
				}
				else if (m.Groups["type"].Value.ToLower() == "r")
				{
					if (m.Groups["critical"].Value == "")
					{
						res += NRXFTAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value)).Result;
					}
					else
					{
						res += NRXFTAsync(int.Parse(m.Groups["value"].Value), int.Parse(m.Groups["sides"].Value), int.Parse(m.Groups["critical"].Value)).Result;
					}
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

			foreach(Match m in dxs)
			{
				string res = "";

				if(m.Groups["critical"].Value == "")
				{
					res += NRXFTAsync(int.Parse(m.Groups["value"].Value), 10).Result;
				}
				else
				{
					res += NRXFTAsync(int.Parse(m.Groups["value"].Value), 10, int.Parse(m.Groups["critical"].Value)).Result;
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

			int fix_sum = 0;

			foreach (Match m in fixes)
			{
				Console.WriteLine(m.Groups["fix"].Value);
				fix_sum += int.Parse(m.Groups["fix"].Value);
			}

			send_msg += "\nResult: " + RandomManager.DiceResultHistory.Sum();

			if (fixes.Count != 0)
			{
				send_msg += ' ' + fix_sum.ToString("+ #;- #") + " -> " + (RandomManager.DiceResultHistory.Sum() + fix_sum);
			}

			await Context.Channel.SendMessageAsync(send_msg);
		}


		private async Task NDXAsync(int value, int sides, int fix = 0, int target = 0)
		{
			if (!await RengeCheckAsync(value, sides))
			{
				return;
			}

			List<int> res = new List<int>();
			for (int i = 0; i < value; i++)
			{
				res.Add(RandomManager.Rand(sides));
			}

			string send_msg = Context.User.Mention + '\n';

			send_msg += value + "D" + sides + " -> ";

			send_msg += GenerateDiceRollString(res, res.Count);

			send_msg += " -> " + res.Sum();

			if (fix != 0)
			{
				send_msg += String.Format("{0} ", fix.ToString("+#;-#")) + "-> " + (res.Sum() + fix);
			}
			if (target > 0)
			{
				send_msg += (res.Sum() <= target ? "    **Success**" : "    **Fail**");
			}

			await Context.Channel.SendMessageAsync(send_msg);
		}

		private async Task<string> NDXFTAsync(int value, int sides)
		{
			// 制約チェック
			if (!await RengeCheckAsync(value, sides))
			{
				return "failed";
			}

			// 戻り文字列初期化
			string ret_str = "";

			// 戻り文字列生成
			List<int> res = new List<int>();
			for (int i = 0; i < value; i++)
			{
				res.Add(RandomManager.Rand(sides));
			}

			ret_str += value + "D" + sides + " -> ";

			ret_str += GenerateDiceRollString(res, res.Count);

			ret_str += " -> " + res.Sum();

			return ret_str;
		}

		private async Task NRXAsync(int value, int sides, int critical = 10, int fix = 0)
		{
			if (!await RengeCheckAsync(value, sides))
			{
				return;
			}

			critical = Math.Max(Math.Min(critical, sides), 2);

			// 一回目
			List<int> res = new List<int>();
			for (int i = 0; i < value; i++)
			{
				res.Add(RandomManager.Rand(sides));
			}

			int critical_value = res.Count(n => n >= critical);
			int ctimes = 0;
			int last_max = 0;
			res.Sort();

			string send_msg = Context.User.Mention + '\n';

			send_msg += value + "DX" + (critical != 10 ? ("@" + critical) : "") + " -> ";

			// 2回目以降
			while (res.Count > 0)
			{
				if (critical_value > 0)
				{
					ctimes++;
				}

				send_msg += GenerateDiceRollString(res, res.Count, isCritical: true, critical: critical);

				last_max = res.Max();

				res.Clear();

				for (int i = 0; i < critical_value; i++)
				{
					res.Add(RandomManager.Rand(sides));
				}

				critical_value = res.Count(n => n >= critical);

				send_msg += " -> ";
			}

			send_msg += ctimes * sides + last_max;

			if (fix != 0)
			{
				send_msg += String.Format("{0} ", fix.ToString("+#;-#")) + "-> " + (ctimes * sides + last_max + fix);
			}

			await Context.Channel.SendMessageAsync(send_msg);
		}

		private async Task<string> NRXFTAsync(int value, int sides, int critical = 10)
		{
			critical = Math.Max(Math.Min(critical, sides), 2);

			// 制約チェック
			if (!await RengeCheckAsync(value, sides))
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

				ret_str += " -> ";
			}

			int dice_res = ctimes * sides + last_max;

			ret_str += dice_res;

			RandomManager.DiceResultHistory.Add(dice_res);

			return ret_str;
		}

		private async Task<bool> RengeCheckAsync(int value, int sides)
		{
			if (value > 100)
			{
				await Context.Channel.SendMessageAsync("100 個を超えるダイスは用意できないかな...");
				return false;
			}
			else if (sides > 99999)
			{
				await Context.Channel.SendMessageAsync("99999 面よりも面の数が多いダイスは用意できないね...");
				return false;
			}

			return true;
		}

		private string GenerateDiceRollString(List<int> diceRes, int count, bool isCritical = false, int critical = 0)
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
	}
}
