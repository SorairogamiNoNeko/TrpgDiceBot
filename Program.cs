using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace TrpgDiceBot
{
	class Program
	{
		private static string _settingDataFile = "settingData.txt";

		private DiscordSocketClient _client;
		private CommandService _comannds;
		private CommandHandler _handler;

		static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();
			_comannds = new CommandService();
			_client.Log += Log;

			_handler = new CommandHandler(_client, _comannds);

			await _handler.InitialCommandsAsync();

			await _client.LoginAsync(TokenType.Bot, "NjkxNjg5MDE4OTkxMzEyOTU4.Xnjnow.4rDKViHS9J9y5Uo9OQ0NXLIe9Jg");
			await _client.StartAsync();

			await Task.Delay(-1);
		}
	}
}
