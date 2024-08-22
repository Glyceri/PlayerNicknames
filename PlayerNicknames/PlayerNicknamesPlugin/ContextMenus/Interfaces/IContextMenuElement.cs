using Dalamud.Game.Gui.ContextMenu;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.ContextMenus.Interfaces;

internal interface IContextMenuElement
{
    string? AddonName { get; }
    Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args);
}
