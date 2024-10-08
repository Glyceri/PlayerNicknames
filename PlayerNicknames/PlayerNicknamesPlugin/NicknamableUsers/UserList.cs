using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;

internal class UserList : IUserList
{
    const int NamableUsersArraySize = 100;

    public INamableUser?[] NamableUsers { get; set; } = new INamableUser?[NamableUsersArraySize];
    public INamableUser? LocalPlayer { get => NamableUsers[0]; }

    public INamableUser? GetUser(ulong userId, bool checkActive = false)
    {
        if (userId == 0) return null;

        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive && checkActive) continue;
            if (pUser.ObjectID == userId) return pUser;

            return pUser;
        }
        return null;
    }

    public INamableUser? GetUser(string username, bool checkActive = false)
    {
        if (username.IsNullOrWhitespace()) return null;

        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive && checkActive) continue;
            if (!string.Equals(pUser.Name, username, System.StringComparison.InvariantCultureIgnoreCase)) continue;

            return pUser;
        }
        return null;
    }

    public INamableUser? GetUser(nint user, bool checkActive = false)
    {
        if (user == nint.Zero) return null;
        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive && checkActive) continue;
            if (pUser.Address != user) continue;

            return pUser;
        }
        return null;
    }

    public INamableUser? GetUserFromID(uint userId, bool checkActive = false)
    {
        if (userId == 0) return null;

        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive && checkActive) continue;
            if (pUser.ObjectID != userId) continue;

            return pUser;
        }
        return null;
    }
}
