using Lumina.Excel;
using Lumina.Excel.GeneratedSheets2;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Wrappers;

internal class SheetsWrapper : ISheets
{
    readonly ExcelSheet<World>? worlds;

    public SheetsWrapper(DalamudServices dalamudServices)
    {
        worlds = dalamudServices.DataManager.GetExcelSheet<World>();
    }

    public string? GetWorldName(ushort worldID)
    {
        if (worlds == null) return null;

        World? world = worlds.GetRow(worldID);
        if (world == null) return null;

        return world.InternalName;
    }

    public ushort? GetWorldID(string worldname)
    {
        if (worlds == null) return null;

        foreach(World? world in worlds)
        {
            if (world == null) continue;
            if (!string.Equals(world.InternalName, worldname, System.StringComparison.InvariantCultureIgnoreCase)) continue;

            return (ushort)world.RowId;
        }

        return null;
    }
}
