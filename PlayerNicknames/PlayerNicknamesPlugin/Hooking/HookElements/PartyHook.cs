using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement, IPartyHook
{
    readonly INameDatabase Database;

    public ulong HoveredContentID { get; private set; }
    public string HoveredName { get; private set; } = string.Empty;
    public ushort HoveredHomeworld { get; private set; }

    public PartyHook(DalamudServices services, IUserList userList, IPlayerServices petServices, IDirtyListener dirtyListener, INameDatabase database) : base(services, userList, petServices, dirtyListener)
    {
        Database = database;
    }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "_PartyList", (a, args) => { LifeCycleUpdate(a, args); Refresh(); });
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool CanContinue(AtkUnitBase* baseD) => !(!baseD->IsVisible || !PlayerServices.Configuration.showOnPartyList || baseD == null);

    void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) => Update((AtkUnitBase*)args.Addon);
    void LifeCycleRefreshUpdate(AddonEvent aEvent, AddonArgs args)
    {
        LifeCycleUpdate(aEvent, args);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleRefreshUpdate);
    }

    protected override void Refresh()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "_PartyList", LifeCycleRefreshUpdate);
    }

    void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD)) return;
        SetupPartyList();
        DrawPartyList((AddonPartyList*)baseD);
    }

    ulong[] partyGroup = new ulong[8]; // a party is always 8 in size
    string[] partyName = new string[8];
    ushort[] partyHomeworld = new ushort[8];

    void SetupPartyList()
    {
        bool isCrossParty = IsCrossParty();
        partyGroup = new ulong[8];
        partyName = new string[8];
        partyHomeworld = new ushort[8];

        // We can assume partyGroup is 8 in length, and so is the struct with party members, Thanks SE!
        for (int i = 0; i < partyGroup.Length; i++)
        {
            ulong contentID = 0;
            string name = string.Empty;
            ushort homeworld = 0;
            int ? index;

            if (isCrossParty)
            {
                CrossRealmMember member = InfoProxyCrossRealm.Instance()->CrossRealmGroups[0].GroupMembers[i];
                contentID = member.ContentId;
                name = member.NameString;
                homeworld = (ushort)member.HomeWorld;

                index = GetCrossPartyIndex(contentID);
            }
            else
            {
                PartyMember member = GroupManager.Instance()->MainGroup.PartyMembers[i];
                contentID = member.ContentId;
                name = member.NameString;
                homeworld = member.HomeWorld;

                index = GetNormalPartyIndex(contentID);
            }

            if (index == null) continue;
            if (index < 0 || index >= partyGroup.Length) continue;

            partyGroup[index.Value] = contentID;
            partyName[index.Value] = name;
            partyHomeworld[index.Value] = homeworld;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsCrossParty() => InfoProxyCrossRealm.Instance()->IsCrossRealm > 0 && GroupManager.Instance()->MainGroup.MemberCount < 1;

    int? GetCrossPartyIndex(ulong contentID)
    {
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

    List<INameEntry> shouldRefresh = new List<INameEntry>();

    protected override void OnNameDatabaseChange(INameEntry nameDatabase)
    {
        if (!shouldRefresh.Contains(nameDatabase))
        {
            shouldRefresh.Add(nameDatabase);
        }
        base.OnNameDatabaseChange(nameDatabase);
    }

    void DrawPartyList(AddonPartyList* partyNode)
    {
        bool found = false;
        for (int i = 0; i < partyNode->MemberCount; i++)
        {
            if (i >= partyGroup.Length) continue;

            PartyListMemberStruct member = partyNode->PartyMembers[i];

            if (member.TargetGlow != null && !found)
            {
                if (member.TargetGlow->IsVisible())
                {
                    HoveredContentID = partyGroup[i];
                    HoveredName = partyName[i];
                    HoveredHomeworld = partyHomeworld[i];

                    if (partyNode->TargetedIndex != i)
                    {
                        found = true;
                    }
                }
            }

            if (member.Name == null) continue;
            if (!member.Name->IsVisible()) continue;           

            ulong contentID = partyGroup[i];

            INameDatabaseEntry entry = Database.GetEntry(contentID);

            if (!shouldRefresh.Remove(entry.ActiveEntry))
            {
                if (!entry.IsActive) continue;
            }

            IClippedName clippedName = PlayerServices.ClippedNameDatabase.GetClippedName(entry);
            IClippedName? oldClippedName = PlayerServices.ClippedNameDatabase.FromCustomName(entry);
            if (oldClippedName != null)
            {
                PlayerServices.StringHelper.ReplaceATKString(member.Name, entry.Name, oldClippedName, false);
            }

            string? customName = entry.ActiveEntry.GetName();
            if (customName == null) continue;

            PlayerServices.StringHelper.ReplaceATKString(member.Name, customName, clippedName, true);
        }
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
    }
}
