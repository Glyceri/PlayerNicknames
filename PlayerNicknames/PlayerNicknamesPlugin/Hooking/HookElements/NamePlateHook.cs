using Dalamud.Game.Gui.NamePlate;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
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

    protected override void Refresh() => DalamudServices.NameplateGUI.RequestRedraw();

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

    unsafe void OnSpecificPlateUpdate(INamePlateUpdateHandler handler)
    {
        if (handler.GameObject == null) return;
        nint address = handler.GameObject.Address;

        INamableUser? user = handler.NamePlateKind switch
        {
            NamePlateKind.PlayerCharacter => HandleAsPlayer(address),
            NamePlateKind.EventNpcCompanion => HandleAsCompanion((Companion*)address),
            NamePlateKind.BattleNpcFriendly => HandleAsPet((BattleChara*)address),
            _ => null
        };
        if (user == null) return;

        string? customName = user.CustomName;
        if (customName == null) return;

        string quotedLine = PlayerServices.StringHelper.MakeQuoted(customName);

        if (handler.NamePlateKind == NamePlateKind.PlayerCharacter)
        {
            handler.NameParts.Text = quotedLine;
        }
        else
        {
            handler.TitleParts.Text = quotedLine;
        }
    }

    INamableUser? HandleAsPlayer(nint playerAddress) => UserList.GetUser(playerAddress);
    unsafe INamableUser? HandleAsCompanion(Companion* companion) => UserList.GetUserFromID(companion->CompanionOwnerId);
    unsafe INamableUser? HandleAsPet(BattleChara* battleChara) => UserList.GetUserFromID(battleChara->OwnerId);
  
}
