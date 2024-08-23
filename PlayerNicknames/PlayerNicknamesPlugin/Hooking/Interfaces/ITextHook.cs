using PlayerNicknames.PlayerNicknamesPlugin.Core;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.Hooking.Interfaces;

internal interface ITextHook : IDisposable
{
    bool Faulty { get; }
    void Setup(DalamudServices service, IUserList userList, IPlayerServices petServices, IDirtyListener dirtyListener, string AddonName, uint[] textPos, Func<INameDatabaseEntry, bool> allowedCallback);
}
