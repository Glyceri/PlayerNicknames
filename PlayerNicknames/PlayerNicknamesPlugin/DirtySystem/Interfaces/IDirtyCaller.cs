using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;

internal interface IDirtyCaller
{
    void DirtyName(INameEntry name);
    void DirtyEntry(INameDatabaseEntry databaseEntry);
    void DirtyDatabase(INameDatabase database);
}
