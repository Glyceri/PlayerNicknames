using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System;

namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

internal unsafe interface INamableUser : IDisposable
{
    bool IsActive { get; }
    bool IsLocalPlayer { get; }

    string Name { get; }
    ulong ContentID { get; }
    ushort Homeworld { get; }
    ulong ObjectID { get; }
    INameDatabaseEntry DatabaseEntry { get; }

    nint Address { get; }
    BattleChara* BattleChara { get; }

    public string? CustomName { get; }

    void Set(Pointer<BattleChara> pointer);
}
