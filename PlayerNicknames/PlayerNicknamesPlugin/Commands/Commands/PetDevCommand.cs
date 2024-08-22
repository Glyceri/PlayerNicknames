using PlayerNicknames.PlayerNicknamesPlugin.Commands.Commands.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using PlayerRenamer;

namespace PlayerNicknames.PlayerNicknamesPlugin.Commands.Commands;

internal class PetDevCommand : Command
{
    readonly Configuration Configuration;

    public PetDevCommand(in DalamudServices dalamudServices, in Configuration configuration, in IWindowHandler windowHandler) : base(dalamudServices, windowHandler) 
    { 
        Configuration = configuration;
    }

    public override string CommandCode { get; } = "/playerdev";
    public override string Description { get; } = "Opens the Player Dev Window";
    public override bool ShowInHelp { get; } = false;

    public override void OnCommand(string command, string args)
    {
        if (Configuration.debugModeActive)
        {
            WindowHandler.Open<DevWindow>();
        }
    }
}
