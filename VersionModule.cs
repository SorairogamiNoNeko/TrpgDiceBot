using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Net;

namespace TrpgDiceBot
{
	[Group("version")]
	[Alias("ver")]
	public class VersionModule : ModuleBase<SocketCommandContext>
	{
		private string _thisVersion = "1.20";

		[Command("")]
		public async Task TellVersionAsync()
		{
			WebClient wc = new WebClient();
			string content = wc.DownloadString("https://cyanhair-neko.hatenablog.com/entry/dicebotversion");
			Match m = Regex.Match(content, @"(?i)(?<=\<id\=\""dicebotver\""\>)(.+)(?=\<\/id\>)");
			await Context.Channel.SendMessageAsync("この Bot のバージョンは、**" + _thisVersion + "** です。\n最新バージョンは、**" + m.Value + "** です。");
		}
	}
}
