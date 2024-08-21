using Dalamud.Plugin;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Serialization;
using PlayerNicknames.PlayerNicknamesPlugin.Updating;
using PlayerNicknames.PlayerNicknamesPlugin.Updating.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;

namespace PlayerRenamer;

public sealed class PlayerNicknamesPlugin : IDalamudPlugin
{
    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;

    readonly DirtyHandler DirtyHandler;

    readonly IUserList UserList;
    readonly INameDatabase Database;
    readonly IUpdateHandler UpdateHandler;
    readonly IWindowHandler WindowHandler;

    readonly SaveHandler SaveHandler;

    public PlayerNicknamesPlugin(IDalamudPluginInterface dalamud)
    {
        DalamudServices = DalamudServices.Create(ref dalamud);
        PlayerServices = new PlayerServices(in DalamudServices);

        DirtyHandler = new DirtyHandler();

        UserList = new UserList();
        Database = new NameDatabase(PlayerServices, DirtyHandler);

        UpdateHandler = new UpdateHandler(DalamudServices, PlayerServices, Database, UserList);

        WindowHandler = new WindowHandler(DalamudServices, PlayerServices, Database, UserList, DirtyHandler);



        PlayerServices.Configuration.Initialise(DalamudServices.PlayerNicknamesPlugin, Database);

        SaveHandler = new SaveHandler(PlayerServices.Configuration, DirtyHandler);

        PlayerServices.Configuration.Save();
    }

    public void Dispose()
    {
        UpdateHandler?.Dispose();
        SaveHandler?.Dispose();
        WindowHandler?.Dispose();
    }
}
