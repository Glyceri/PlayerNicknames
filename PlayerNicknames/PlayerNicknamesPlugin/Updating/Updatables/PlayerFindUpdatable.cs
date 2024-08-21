using Dalamud.Plugin.Services;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Updating.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;

namespace PlayerNicknames.PlayerNicknamesPlugin.Updating.Updatables;

internal unsafe class PlayerFindUpdatable : IUpdatable
{
    readonly DalamudServices DalamudServices;
    readonly IPlayerServices PlayerServices;
    readonly INameDatabase Database;
    readonly IUserList UserList;

    public PlayerFindUpdatable(DalamudServices dalamudServices, IPlayerServices playerServices, INameDatabase database, IUserList userList)
    {
        DalamudServices = dalamudServices;
        PlayerServices = playerServices;
        Database = database;
        UserList = userList;
    }

    public void OnUpdate(IFramework framework)
    {
        Span<Pointer<BattleChara>> charaSpan = CharacterManager.Instance()->BattleCharas;
        int charaSpanLength = charaSpan.Length;

        for (int i = 0; i < charaSpanLength; i++)
        {
            Pointer<BattleChara> battleChara = charaSpan[i];
            INamableUser? namableUser = UserList.NamableUsers[i];

            ObjectKind currentObjectKind = ObjectKind.None;
            ulong namableContentID = ulong.MaxValue;
            ulong contentID = ulong.MaxValue;

            if (battleChara != null)
            {
                contentID = battleChara.Value->ContentId;
                currentObjectKind = battleChara.Value->GetObjectKind();
            }

            if (namableUser != null) namableContentID = namableUser.ContentID;

            if (contentID == ulong.MaxValue || contentID == 0 || namableContentID != contentID)
            {
                // Destroy the user
                UserList.NamableUsers[i]?.Dispose();
                UserList.NamableUsers[i] = null;
            }

            if (namableUser == null && battleChara != null && currentObjectKind == ObjectKind.Pc)
            {
                // Create a user
                INamableUser newUser = new NamableUser(PlayerServices, Database, battleChara);
                UserList.NamableUsers[i] = newUser;
                continue;
            }

            namableUser?.Set(battleChara);
        }
    }
}
