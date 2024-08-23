using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Gui.ContextMenu;
using PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.ContextMenuElements;

// This one will most likely be removed!
internal class TargetContextMenu : IContextMenuElement
{
    // Null means context menu didn't come from an addon
    public string? AddonName { get; } = null;

    readonly DalamudServices DalamudServices;
    readonly IUserList UserList;
    readonly IWindowHandler WindowHandler;

    public TargetContextMenu(in DalamudServices dalamudServices, in IUserList userList, in IWindowHandler windowHandler)
    {
        DalamudServices = dalamudServices;
        UserList = userList;
        WindowHandler = windowHandler;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        INamableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) { DalamudServices.PluginLog.Debug("Local User null!"); return null; }

        IGameObject? target = DalamudServices.TargetManager.Target;
        if (target == null) { DalamudServices.PluginLog.Debug("Target null!"); return null; }

        INamableUser? targetUser = UserList.GetUser(target.Address);
        if (targetUser == null) { DalamudServices.PluginLog.Debug("targetUser null!"); return null; }
        if (targetUser == localUser) { DalamudServices.PluginLog.Debug("targetUser == localuser"); return null; }

        return (a) => WindowHandler.GetWindow<RenameWindow>()?.SetRenameWindow(targetUser.DatabaseEntry, true);
    }
}
