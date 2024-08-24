using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PlayerNicknames.PlayerNicknamesPlugin.Services.ServiceWrappers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PlayerNicknames.PlayerNicknamesPlugin.Services.ServiceWrappers;

internal class StringHelperWrapper : IStringHelper
{
    readonly ISheets Sheets;

    //const string leftQuote = "》";
    //const string rightQuote = "《";

    const string leftQuote = "„";
    const string rightQuote = "“";

    public StringHelperWrapper(ISheets sheets)
    {
        Sheets = sheets;
    }

    public unsafe string? ReplaceATKString(AtkTextNode* textNode, string newCustomName, IClippedName clippedName, bool displayAsNickname)
    {
        if (textNode == null) return string.Empty;
        string baseText = textNode->NodeText.ToString();

        string newString = DoReplacePart(baseText, newCustomName, clippedName, displayAsNickname);
        textNode->SetText(newString);
        return newString;
    }

    public string SetUtf8String(in Utf8String utf8String, string text)
    {
        string newString = text;

        utf8String.SetString(newString);

        return text;
    }

    public unsafe string SetATKString(AtkTextNode* atkNode, string text)
    {
        string newString = text;

        byte[] data = Encoding.UTF8.GetBytes(newString);

        atkNode->SetText(data);

        return text;
    }

    public void ReplaceSeString(ref SeString message, string newCustomName, IClippedName clippedName)
    {
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            string curString = tPayload.Text!.ToString();
            tPayload.Text = ReplaceStringPart(curString, newCustomName, clippedName);

            message.Payloads[i] = tPayload;
        }
    }

    public string ReplaceStringPart(string baseString, string newCustomName, IClippedName clippedName)
    {
        return DoReplacePart(baseString, newCustomName, clippedName, true);
    }

    public string DoReplacePart(string baseString, string newCustomName, IClippedName clippedName, bool displayAsNickname)
    {
        int length = clippedName.Length;
        for (int i = 0; i < length; i++)
        {
            baseString = baseString.Replace("[", @"^\[").Replace("]", @"^\]\");
            string regString = clippedName[i];
            regString = $"\\b" + regString + "\\b";
            baseString = Regex.Replace(baseString, regString, MakeString(PluginConstants.forbiddenCharacter, i + 1), RegexOptions.IgnoreCase);
        }

        if (displayAsNickname)
        {
            newCustomName = MakeQuoted(newCustomName);
        }

        if (baseString.Contains('^'))
        {
            baseString = baseString.Replace(leftQuote, string.Empty);
            baseString = baseString.Replace(rightQuote, string.Empty);
        }

        for (int i = length - 1; i >= 0; i--)
        {
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, i + 1), newCustomName);
        }

        return baseString;
    }

    string MakeString(char c, int count) => new string(c, count);

    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public string MakeQuoted(string baseString) => $"{leftQuote}{baseString}{rightQuote}";

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
