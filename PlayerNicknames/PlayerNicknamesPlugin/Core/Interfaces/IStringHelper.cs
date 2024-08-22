using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    string MakeTitleCase(string str);
    /// <summary>
    /// A sticky player string is as follows "FirstnameLastnameWorldname"
    /// </summary>
    /// <param name="str">Sticky Player String</param>
    /// <returns>A player struct if it can parse it</returns>
    PlayerStruct? ParseStickyPlayerString(string str);
}
