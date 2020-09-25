using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace TrpgDiceBot
{
	public class CommandHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;

		public CommandHandler(DiscordSocketClient client, CommandService commands)
		{
			_client = client;
			_commands = commands;
		}

		public async Task InitialCommandsAsync()
		{
			_client.MessageReceived += HandleCommandAsync;

			await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
		}

		private async Task HandleCommandAsync(SocketMessage messageParam)
		{
			var message = messageParam as SocketUserMessage;

			if(message == null)
			{
				return;
			}

			int arg_pos = 0;

			if(!(message.HasCharPrefix('%', ref arg_pos) || message.HasMentionPrefix(_client.CurrentUser, ref arg_pos))
				|| message.Author.IsBot)
			{
				if (message.HasCharPrefix('&', ref arg_pos))
				{
					// ひとつの文字列として読み取る場合
					await DiceRoll.Execute(message);
				}

				return;
			}

			var context = new SocketCommandContext(_client, message);

			var result = await _commands.ExecuteAsync(context: context, argPos: arg_pos, services: null);

			if (!result.IsSuccess)
			{
				await context.Channel.SendMessageAsync(result.ErrorReason);
			}
		}
	}
}
