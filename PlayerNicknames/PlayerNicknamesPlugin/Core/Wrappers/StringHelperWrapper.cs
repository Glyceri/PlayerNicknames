using Dalamud.Utility;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class StringHelperWrapper : IStringHelper
{
    readonly ISheets Sheets;

    public StringHelperWrapper(ISheets sheets)
    {
        Sheets = sheets;
    }

    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public PlayerStruct? ParseStickyPlayerString(string str)
    {
        if (str.IsNullOrWhitespace()) return null;

        // What this does is add a space in front of every capital letter, given that names show like Firstname LastnameWorldname
        // we can now get Firstname Lastname Worldname out of that instead
        str = Regex.Replace(str, "(?<![\\sA-Z])([A-Z])", " $1").Trim();
        if (str.IsNullOrWhitespace()) return null;

        string[] parts = str.Split(' ');
        if (parts.Length != 3) return null;

        string username = $"{parts[0]} {parts[1]}";
        string homeworldName = parts[2];

        ushort? homeworldID = Sheets.GetWorldID(homeworldName);
        if (homeworldID == null) return null;

        return new PlayerStruct(username, homeworldID.Value);
    }
}
