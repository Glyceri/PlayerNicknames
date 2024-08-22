using PlayerNicknames.PlayerNicknamesPlugin.Commands.Commands;
using PlayerNicknames.PlayerNicknamesPlugin.Commands.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerRenamer;
using System.Collections.Generic;

namespace PlayerNicknames.PlayerNicknamesPlugin.Commands;

internal class CommandHandler : ICommandHandler
{
    readonly DalamudServices DalamudServices;
    readonly IWindowHandler WindowHandler;
    readonly Configuration Configuration;

    readonly List<ICommand> Commands = new List<ICommand>();

    public CommandHandler(in DalamudServices dalamudServices, in Configuration configuration, in IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        Configuration = configuration;
        WindowHandler = windowHandler;

        RegisterCommands();
    }

    void RegisterCommands()
    {
        RegisterCommand(new PlayerNameCommand      (in DalamudServices, in WindowHandler));
        RegisterCommand(new PlayerSettingsCommand  (in DalamudServices, in WindowHandler));
        RegisterCommand(new PlayerListCommand      (in DalamudServices, in WindowHandler));
        RegisterCommand(new PetDevCommand       (in DalamudServices, in Configuration, in WindowHandler));
    }

    void RegisterCommand(ICommand command)
    {
        Commands.Add(command);
    }

    public void Dispose()
    {
        foreach(ICommand command in Commands)
        {
            command?.Dispose();
        }   
        Commands.Clear();
    }
}
