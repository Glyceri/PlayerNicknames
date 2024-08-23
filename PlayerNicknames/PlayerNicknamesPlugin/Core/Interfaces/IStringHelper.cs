using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;

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

    string? ReplaceATKString(AtkTextNode* textNode, INameDatabaseEntry databaseEntry, bool checkForEmptySpace = true);
    string? ReplaceStringPart(string baseString, INameDatabaseEntry databaseEntry, bool checkForEmptySpaces = true);
    string DoReplacePart(string baseString, string baseName, string customName, bool checkForEmptySpaces);
    void ReplaceSeString(ref SeString message, INameDatabaseEntry databaseEntry, bool checkForEmptySpace = true);
    string SetATKString(AtkTextNode* atkNode, string text);
    string SetUtf8String(in Utf8String utf8String, string text);
}
