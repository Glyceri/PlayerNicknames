using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Database;

internal class NameEntry : INameEntry
{
    string? name;

    readonly IDirtyCaller DirtyCaller;

    public NameEntry(IDirtyCaller dirtyCaller, string? name)
    {
        DirtyCaller = dirtyCaller;
        this.name = name;
    }

    public string? GetName()
    {
        return name;
    }

    public void SetName(string? name)
    {
        if (this.name == name) return;

        this.name = name;
        SetDirty();
    }

    void SetDirty()
    {
        DirtyCaller.DirtyName(this);
    }
}
