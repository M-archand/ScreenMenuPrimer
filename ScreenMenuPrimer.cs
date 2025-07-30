using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CS2MenuManager.API.Class;
using CS2MenuManager.API.Enum;
using CS2MenuManager.API.Menu;

namespace ScreenMenuPrimer
{
    public class Plugin : BasePlugin
    {
        public override string ModuleName => "CS2MenuManager - ScreenMenu Primer";
        public override string ModuleAuthor => "Marchand";
        public override string ModuleVersion => "1.0.0";

        private static readonly HashSet<ulong> PrimedPlayers = new();

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            RegisterEventHandler<EventPlayerTeam>((@event, info) =>
            {
                var player = @event.Userid;

                if (player == null || !player.IsValid || player.IsBot)
                    return HookResult.Continue;

                var steamId = player.SteamID;

                if (@event.Team != 2 && @event.Team != 3)
                    return HookResult.Continue;

                if (PrimedPlayers.Contains(steamId))
                    return HookResult.Continue;

                PrimedPlayers.Add(steamId);

                Server.NextFrame(() =>
                {
                    var dummyMenu = new ScreenMenu("", this)
                    {
                        ExitButton = false,
                        ScreenMenu_FreezePlayer = false,
                        ScreenMenu_ShowResolutionsOption = false,
                        ScreenMenu_Size = 1,
                        ScreenMenu_MenuType = MenuType.KeyPress
                    };

                    dummyMenu.AddItem("", (p, _) => { });
                    dummyMenu.Display(player, 0);

                    Server.NextFrame(() => { MenuManager.CloseActiveMenu(player); });
                });

                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
            {
                var player = @event.Userid;

                if (player == null || !player.IsValid)
                    return HookResult.Continue;

                var steamId = player.SteamID;
                PrimedPlayers.Remove(steamId);

                return HookResult.Continue;
            });
        }
    }
}