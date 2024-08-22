using PlayerNicknames.PlayerNicknamesPlugin.Commands.Commands.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;

namespace PlayerNicknames.PlayerNicknamesPlugin.Commands.Commands;

internal class PlayerNameCommand : Command
{
    public PlayerNameCommand(in DalamudServices dalamudServices, in IWindowHandler windowHandler) : base(dalamudServices, windowHandler) { }

    public override string CommandCode { get; } = "/playername";
    public override string Description { get; } = "Opens the Player Rename Window.";
    public override bool ShowInHelp { get; } = true;

    public override void OnCommand(string command, string args)
    {
        WindowHandler.Open<RenameWindow>();
    }
}
