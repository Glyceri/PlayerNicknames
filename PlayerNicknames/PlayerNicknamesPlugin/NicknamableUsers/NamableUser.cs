using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;

internal unsafe class NamableUser : INamableUser
{
    public bool IsActive => DatabaseEntry.IsActive;
    public bool IsLocalPlayer { get; private set; }

    public string Name { get; private set; }
    public ulong ContentID { get; private set; }
    public ushort Homeworld { get; private set; }
    public ulong ObjectID { get; private set; }
    public INameDatabaseEntry DatabaseEntry { get; private set; }
    public nint Address { get; private set; }
    public unsafe BattleChara* BattleChara { get; private set; }
    public string? CustomName => DatabaseEntry.ActiveEntry.GetName();

    readonly IPlayerServices PlayerServices;

    public NamableUser(IPlayerServices playerServices, INameDatabase database, Pointer<BattleChara> battleChara)
    {
        PlayerServices = playerServices;

        BattleChara = battleChara.Value;
        Address = (nint)BattleChara;

        IsLocalPlayer = BattleChara->ObjectIndex == 0;
        Name = BattleChara->NameString;
        ContentID = BattleChara->ContentId;
        Homeworld = BattleChara->HomeWorld;

        ObjectID = BattleChara->GetGameObjectId();
        DatabaseEntry = database.GetEntry(ContentID);
        DatabaseEntry.UpdateEntry(this);
    }

    public void Set(Pointer<BattleChara> pointer)
    {
        Address = (nint)pointer.Value;
        BattleChara = pointer.Value;
    }

    public void Dispose()
    {
        
    }
}
