namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

internal interface IUserList
{
    INamableUser?[] NamableUsers { get; }
    INamableUser? LocalPlayer { get; }

    INamableUser? GetUserFromID(uint userId, bool checkActive = false); 
    INamableUser? GetUser(ulong userId, bool checkActive = false);
    INamableUser? GetUser(string username, bool checkActive = false);
    INamableUser? GetUser(nint user, bool checkActive = false);
}
