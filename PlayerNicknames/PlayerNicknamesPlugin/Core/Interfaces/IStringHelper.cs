using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;

namespace PlayerNicknames.PlayerNicknamesPlugin.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    string MakeTitleCase(string str);
    /// <summary>
    /// A sticky player string is as follows "FirstnameLastnameWorldname"
    /// </summary>
    /// <param name="str">Sticky Player String</param>
    /// <returns>A player struct if it can parse it</returns>
    PlayerStruct? ParseStickyPlayerString(string str);

    string? ReplaceATKString(AtkTextNode* textNode, string newCustomName, IClippedName clippedName, bool displayAsNickname);
    string? ReplaceStringPart(string baseString, string newCustomName, IClippedName clippedName);
    string DoReplacePart(string baseString, string newCustomName, IClippedName clippedName, bool displayAsNickname);
    void ReplaceSeString(ref SeString message, string newCustomName, IClippedName clippedName);
    string SetATKString(AtkTextNode* atkNode, string text);
    string SetUtf8String(in Utf8String utf8String, string text);
    string MakeQuoted(string baseString);
}
