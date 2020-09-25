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
	[Group("help")]
	public class HelpModule : ModuleBase<SocketCommandContext>
	{
		[Command("")]
		public async Task HelpAsync()
		{
			await Context.Channel.SendMessageAsync(
				"この Bot に用意されているコマンドは以下の通りです。\n" +
				"`%help`\n" +
				"この文章を呼び出すコマンド。\n" +
				"`%version` `%ver`\n" +
				"バージョンの情報を表示します。\n" +
				"\n" +
				"ダイスについて\n" +
				"ダイスは先頭に `&` をつけることで利用できます。\n" +
				"例: `&1d6`, `&1d10 - 1d3` など\n" +
				"また、`&p` でとりあえず 1d100 を振れます。\n" +
				"その横に数字を置くと、その数字の値を技能値とした時に成功したか、失敗したかも表示されます。\n" +
				"例: `&p`, `&p50` など"
				);
		}
	}
}
