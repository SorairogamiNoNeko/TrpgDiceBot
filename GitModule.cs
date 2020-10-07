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
	[Group("git")]
	public class GitModule : ModuleBase<SocketCommandContext>
	{
		[Command("")]
		public async Task SendGitAsync()
		{
			await Context.Channel.SendMessageAsync("この Bot のソースコードがある git です。\n" +
				"https://github.com/SorairogamiNoNeko/TrpgDiceBot");
		}
	}
}
