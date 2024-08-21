using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.NicknamableUsers;

internal unsafe class NamableUser : INamableUser
{
    public bool IsActive { get; }
    public bool IsLocalPlayer { get; }

    public string Name { get; }
    public ulong ContentID { get; }
    public ushort Homeworld { get; }
    public ulong ObjectID { get; }
    public INameDatabaseEntry DatabaseEntry { get; }
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

        if (IsLocalPlayer) DatabaseEntry.UpdateContentID(ContentID);
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
