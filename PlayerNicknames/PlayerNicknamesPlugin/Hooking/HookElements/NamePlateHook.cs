using Dalamud.Game.Gui.NamePlate;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using System.Collections.Generic;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.HookElements;

internal class NamePlateHook : HookableElement
{
    public NamePlateHook(DalamudServices services, IPlayerServices petServices, IUserList pettableUserList, IDirtyListener dirtyListener) : base(services, pettableUserList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.NameplateGUI.OnNamePlateUpdate += OnPlateUpdate;
        Refresh();
    }

    protected override void OnDispose()
    {
        Refresh();
        DalamudServices.NameplateGUI.OnNamePlateUpdate -= OnPlateUpdate;
    }

    protected override void OnNameDatabaseChange(INameEntry nameDatabase) => Refresh();
    protected override void OnPettableDatabaseChange(INameDatabase pettableDatabase) => Refresh();
    protected override void OnPettableEntryChange(INameDatabaseEntry pettableEntry) => Refresh();

    void Refresh()
    {
        DalamudServices.NameplateGUI.RequestRedraw();
    }

    void OnPlateUpdate(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        if (!PlayerServices.Configuration.showOnNameplates) return;

        int size = handlers.Count;
        
        for (int i = 0; i < size; i++)
        {
            INamePlateUpdateHandler handler = handlers[i];
            OnSpecificPlateUpdate(handler);
        }
    }

    void OnSpecificPlateUpdate(INamePlateUpdateHandler handler)
    {
        if (handler.NamePlateKind != NamePlateKind.PlayerCharacter) return;

        if (handler.GameObject == null) return;
        nint address = handler.GameObject.Address;

        INamableUser? user = UserList.GetUser(address);
        if (user == null) return;

        string? customName = user.CustomName;
        if (customName == null) return;

        handler.NameParts.Text = PlayerServices.StringHelper.MakeQuoted(customName);
    }
}
