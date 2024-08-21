using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;

internal class UserList : IUserList
{
    const int NamableUsersArraySize = 100;

    public INamableUser?[] NamableUsers { get; set; } = new INamableUser?[NamableUsersArraySize];
    public INamableUser? LocalPlayer { get => NamableUsers[0]; }

    public INamableUser? GetUser(ulong userId)
    {
        if (userId == 0) return null;

        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (pUser.ObjectID == userId) return pUser;

            return pUser;
        }
        return null;
    }

    public INamableUser? GetUser(string username)
    {
        if (username.IsNullOrWhitespace()) return null;

        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (!string.Equals(pUser.Name, username, System.StringComparison.InvariantCultureIgnoreCase)) continue;

            return pUser;
        }
        return null;
    }

    public INamableUser? GetUser(nint user)
    {
        if (user == nint.Zero) return null;
        for (int i = 0; i < NamableUsersArraySize; i++)
        {
            INamableUser? pUser = NamableUsers[i];
            if (pUser == null) continue;
            if (!pUser.IsActive) continue;
            if (pUser.Address != user) continue;

            return pUser;
        }
        return null;
    }
}
