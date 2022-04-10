using Discord.Interactions;

namespace DiscordBot.Modules
{
    public class EchoModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("echo", "Echoes back the provided argument")]
        public async Task Echo(string text)
        {
            await RespondAsync(text);
        }
    }
}
