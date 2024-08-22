using Dalamud.Interface.Windowing;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.ImageDatabase.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Base;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Windowing.Windows;
using System.Linq;

namespace PlayerNicknames.PlayerNicknamesPlugin.Windowing;

internal class WindowHandler : IWindowHandler
{
    static int _internalCounter = 0;
    public static int InternalCounter { get => _internalCounter++; }

    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;
    readonly IUserList UserList;
    readonly IDirtyListener DirtyListener;
    readonly INameDatabase Database;
    readonly IImageDatabase ImageDatabase;

    readonly WindowSystem WindowSystem;

    public WindowHandler(DalamudServices dalamudServices, IPlayerServices playerServices, INameDatabase database, IUserList userList, IDirtyListener dirtyListener, IImageDatabase imageDatabase)
    {
        WindowSystem = new WindowSystem("Player Nicknames");

        DalamudServices = dalamudServices;
        PlayerServices = playerServices;
        Database = database;
        UserList = userList;
        DirtyListener = dirtyListener;
        ImageDatabase = imageDatabase;

        DirtyListener.RegisterOnDirtyDatabaseEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyName(HandleDirty);

        DalamudServices.PlayerNicknamesPlugin.UiBuilder.Draw += Draw;

        Register();
    }

    void Register()
    {
        AddWindow(new RenameWindow(this, DalamudServices, PlayerServices, UserList, ImageDatabase));
        AddWindow(new DevWindow(this, DalamudServices, PlayerServices.Configuration, UserList));
        AddWindow(new KofiWindow(this, DalamudServices, PlayerServices.Configuration));
    }

    void AddWindow(PlayerWindow window)
    {
        WindowSystem.AddWindow(window);
    }

    bool isDirty = false;

    void HandleDirty(INameEntry nameEntry)
    {
        isDirty = true;
    }

    void HandleDirty(INameDatabaseEntry entry)
    {
        isDirty = true;
    }

    void Draw()
    {
        _internalCounter = 0;
        WindowSystem.Draw();

        if (isDirty)
        {
            HandleDirty();
            isDirty = false;
        }
    }

    void HandleDirty()
    {
        foreach (IPWindow window in WindowSystem.Windows.Cast<PlayerWindow>())
        {
            DalamudServices.Framework.Run(window.NotifyDirty);
        }
    }

    public void Open<T>() where T : IPWindow
    {
        foreach (IPWindow window in WindowSystem.Windows.Cast<PlayerWindow>())
        {
            if (window is not T tWindow) continue;
            tWindow.Open();
        }
    }

    public void Close<T>() where T : IPWindow
    {
        foreach (IPWindow window in WindowSystem.Windows.Cast<PlayerWindow>())
        {
            if (window is not T tWindow) continue;
            tWindow.Close();
        }
    }

    public T? GetWindow<T>() where T : IPWindow
    {
        return WindowSystem.Windows.OfType<T>().FirstOrDefault();
    }

    public void Toggle<T>() where T : IPWindow
    {
        foreach (IPWindow window in WindowSystem.Windows.Cast<PlayerWindow>())
        {
            if (window is not T tWindow) continue;
            tWindow.Toggle();
        }
    }

    public void Dispose()
    {
        DalamudServices.PlayerNicknamesPlugin.UiBuilder.Draw -= Draw;

        foreach (IPWindow window in WindowSystem.Windows.Cast<PlayerWindow>())
        {
            window?.Dispose();
        }

        DirtyListener.RegisterOnDirtyDatabaseEntry(HandleDirty);
        DirtyListener.RegisterOnDirtyName(HandleDirty);

        WindowSystem?.RemoveAllWindows();
    }
}
