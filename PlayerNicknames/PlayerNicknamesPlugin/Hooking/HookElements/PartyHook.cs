using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using System;
using System.Runtime.CompilerServices;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement
{
    readonly INameDatabase Database;

    public PartyHook(DalamudServices services, IUserList userList, IPlayerServices petServices, IDirtyListener dirtyListener, INameDatabase database) : base(services, userList, petServices, dirtyListener)
    {
        Database = database;
    }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "_PartyList", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool CanContinue(AtkUnitBase* baseD) => !(!baseD->IsVisible || !PlayerServices.Configuration.showOnPartyList || baseD == null);

    void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) => Update((AtkUnitBase*)args.Addon);

    void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD)) return;
        SetupPartyList();
        DrawPartyList((AddonPartyList*)baseD);
    }

    ulong[] partyGroup = new ulong[8]; // a party is always 8 in size
    string[] lastEdited = new string[8];

    void SetupPartyList()
    {
        GroupManager* gManager = (GroupManager*)DalamudServices.PartyList.GroupManagerAddress;
        if (gManager == null) return;

        bool isCrossParty = IsCrossParty();
        partyGroup = new ulong[8];

        // We can assume partyGroup is 8 in length, and so is the struct with party members, Thanks SE!
        for (int i = 0; i < partyGroup.Length; i++)
        {
            PartyMember member = gManager->MainGroup.PartyMembers[i];
            ulong contentID = member.ContentId;

            int? index;

            if (isCrossParty) index = GetCrossPartyIndex(contentID);
            else index = GetNormalPartyIndex(contentID);

            if (index == null) continue;
            if (index < 0 || index >= partyGroup.Length) continue;

            partyGroup[index.Value] = contentID;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsCrossParty() => InfoProxyCrossRealm.Instance()->IsCrossRealm > 0 && GroupManager.Instance()->MainGroup.MemberCount < 1;

    int? GetCrossPartyIndex(ulong contentID)
    {
        if (InfoProxyCrossRealm.Instance() == null) return null;

        CrossRealmMember* member = InfoProxyCrossRealm.GetMemberByContentId(contentID);
        if (member == null) return null;

        return member->MemberIndex;
    }

    int? GetNormalPartyIndex(ulong contentID)
    {
        int memberCount = GroupManager.Instance()->MainGroup.MemberCount;
        bool foundSelf = false;

        ulong localContentID = UserList.LocalPlayer?.ContentID ?? 0;

        for (int i = 0; i < memberCount; i++)
        {
            int actualCurrent = i;
            if (!foundSelf)
            {
                actualCurrent++;
            }

            PartyMember* member = GroupManager.Instance()->MainGroup.GetPartyMemberByIndex(i);
            if (member->ContentId == localContentID)
            {
                foundSelf = true;
                actualCurrent = 0;
            }

            if (contentID == member->ContentId)
            {
                return actualCurrent;
            }
        }
        return null;
    }

    bool refresh = false;

    protected override void OnNameDatabaseChange(INameEntry nameDatabase)
    {
        refresh = true;
    }

    void DrawPartyList(AddonPartyList* partyNode)
    {
        if (refresh)
        {
            
            refresh = false;
        }

        for (int i = 0; i < partyNode->MemberCount; i++)
        {
            if (i >= partyGroup.Length) continue;

            PartyListMemberStruct member = partyNode->PartyMembers[i];
            if (member.Name == null) continue;
            if (!member.Name->IsVisible()) continue;

            ulong contentID = partyGroup[i];

            INameDatabaseEntry entry = Database.GetEntry(contentID);


            PlayerServices.StringHelper.SetATKString(member.Name, PlayerServices.StringHelper.DoReplacePart(member.Name->NodeText.ToString(), lastEdited[i], entry.Name, false));
            lastEdited[i] = entry.ActiveEntry.GetName() ?? entry.Name;
            PlayerServices.StringHelper.ReplaceATKString(member.Name, entry, false);
        }
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
    }
}
