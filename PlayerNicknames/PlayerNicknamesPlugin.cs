using Dalamud.Plugin;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Lodestone;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Serialization;
using PlayerNicknames.PlayerNicknamesPlugin.Updating;
using PlayerNicknames.PlayerNicknamesPlugin.Updating.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.ContextMenus;
using PlayerNicknames.PlayerNicknamesPlugin.Commands;
using PlayerNicknames.PlayerNicknamesPlugin.Commands.Interfaces;

namespace PlayerRenamer;

public sealed class PlayerNicknamesPlugin : IDalamudPlugin
{
    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;

    readonly DirtyHandler DirtyHandler;

    readonly IImageDatabase ImageDatabase;
    readonly IUserList UserList;
    readonly INameDatabase Database;
    readonly IUpdateHandler UpdateHandler;
    readonly IWindowHandler WindowHandler;

    readonly ICommandHandler CommandHandler;
    readonly LodestoneNetworker LodestoneNetworker;
    readonly ILodestoneNetworker LodestoneNetworkerInterface;
    readonly SaveHandler SaveHandler;
    readonly ContextMenuHandler ContextMenu;

    public PlayerNicknamesPlugin(IDalamudPluginInterface dalamud)
    {
        DalamudServices = DalamudServices.Create(ref dalamud);
        PlayerServices = new PlayerServices(in DalamudServices);

        LodestoneNetworkerInterface = LodestoneNetworker = new LodestoneNetworker();

        DirtyHandler = new DirtyHandler();

        UserList = new UserList();
        Database = new NameDatabase(PlayerServices, DirtyHandler);

        ImageDatabase = new ImageDatabase(in DalamudServices, PlayerServices, LodestoneNetworkerInterface);

        UpdateHandler = new UpdateHandler(DalamudServices, PlayerServices, Database, UserList, LodestoneNetworker, ImageDatabase);
        WindowHandler = new WindowHandler(DalamudServices, PlayerServices, Database, UserList, DirtyHandler, ImageDatabase);

        CommandHandler = new CommandHandler(DalamudServices, PlayerServices.Configuration, WindowHandler);
        ContextMenu = new ContextMenuHandler(in DalamudServices, in PlayerServices, in UserList, in WindowHandler, Database);

        PlayerServices.Configuration.Initialise(DalamudServices.PlayerNicknamesPlugin, Database);
        SaveHandler = new SaveHandler(PlayerServices.Configuration, DirtyHandler);
    }

    public void Dispose()
    {
        CommandHandler?.Dispose();
        ContextMenu?.Dispose();
        LodestoneNetworker?.Dispose();
        ImageDatabase?.Dispose();
        UpdateHandler?.Dispose();
        WindowHandler?.Dispose();
        SaveHandler?.Dispose();
    }
}
