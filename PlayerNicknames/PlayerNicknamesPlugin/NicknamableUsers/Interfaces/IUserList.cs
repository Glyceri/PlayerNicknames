namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

internal interface IUserList
{
    INamableUser?[] NamableUsers { get; }
    INamableUser? LocalPlayer { get; }

    INamableUser? GetUser(ulong userId);
    INamableUser? GetUser(string username);
    INamableUser? GetUser(nint user);
}
