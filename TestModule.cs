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
	[Group("test")]
	[Alias("t")]
	public class TestModule : ModuleBase<SocketCommandContext>
	{
		[Command("regex")]
		[Alias("rg")]
		public async Task RegexTestAsync(string format)
		{
			MatchCollection dxs = Regex.Matches(format, @"(?i)(\+|\-|)(?<value>\d+)dx(@(?<critical>\d+))?");

			string send_msg = "";

			foreach(Match m in dxs)
			{
				send_msg += "value    : " + m.Groups["value"].Value + '\n';

				send_msg += "critical : " + m.Groups["critical"].Value;
			}

			await Context.Channel.SendMessageAsync(send_msg);
		}
	}
}
