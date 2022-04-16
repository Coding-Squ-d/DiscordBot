using Discord.Interactions;

namespace DiscordBot.Modules
{
    public class TutorModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("tutor", "Need tutoring help? Run this command")]
        public async Task Tutor()
        {
            await RespondAsync("Your tutor link is: ");
        }
    }
}
