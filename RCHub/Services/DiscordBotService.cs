using DSharpPlus.Entities;
using DSharpPlus;

namespace RCHub.Services
{
    public class DiscordBotService
    {
        private readonly IConfiguration _configuration;
        private readonly DiscordClient _discordClient;

        public DiscordBotService(IConfiguration configuration)
        {
            _configuration = configuration;

            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = ConfigurationBinder.GetValue<string>(_configuration, "DISCORD_BOTTOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            _discordClient.ConnectAsync();

        }

        public async Task<DiscordUser> GetDiscordUserAsync(ulong id)
        {
            var user = await _discordClient.GetUserAsync(id);
            return user;
        }

        public async Task<DiscordMember?> GetServerMemberAsync(ulong id)
        {
            var discordServer = await _discordClient.GetGuildAsync(ulong.Parse(ConfigurationBinder.GetValue<string>(_configuration, "DISCORD_SERVERID")));
            return await discordServer.GetMemberAsync(id);
        }

        //        public async Task SendClanReviewNotificationAsync(string? userId, string? clanId)
        //        {
        //            try
        //            {
        //                var defaultChannelId = ConfigurationBinder.GetValue<ulong>(_configuration, "DISCORD_DEFAULTCHANNELID");
        //                var channel = await _discordClient.GetChannelAsync(defaultChannelId);

        //#if (DEBUG)
        //                var link = "https://localhost:32770/Administration/Clans/Details?id=" + clanId;
        //#else
        //            var link = "https://preview.territorial-hq.com/Administration/Clans/Details?id=" + clanId;
        //#endif
        //                await _discordClient.SendMessageAsync(channel, "User " + userId + " has marked a clan listing for review: " + link);
        //            }
        //            catch
        //            {

        //            }
        //        }

        //        public async Task SendEventReviewNotificationAsync(string? userId, string? eventId)
        //        {
        //            try
        //            {
        //                var defaultChannelId = ConfigurationBinder.GetValue<ulong>(_configuration, "DISCORD_DEFAULTCHANNELID");
        //                var channel = await _discordClient.GetChannelAsync(defaultChannelId);

        //#if (DEBUG)
        //                var link = "https://localhost:32770/Administration/CommunityEvents/Details?id=" + eventId;
        //#else
        //            var link = "https://preview.territorial-hq.com/Administration/CommunityEvents/Details?id=" + eventId;
        //#endif
        //                await _discordClient.SendMessageAsync(channel, "User " + userId + " has marked an event for review: " + link);
        //            }
        //            catch
        //            {

        //            }
        //        }
    }
}