using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Struct;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class StringHelperWrapper : IStringHelper
{
    readonly ISheets Sheets;

    public StringHelperWrapper(ISheets sheets)
    {
        Sheets = sheets;
    }

    public unsafe string? ReplaceATKString(AtkTextNode* textNode, INameDatabaseEntry databaseEntry, bool checkForEmptySpace = true)
    {
        if (textNode == null) return string.Empty;
        string baseText = textNode->NodeText.ToString();
        string newString = ReplaceStringPart(baseText, databaseEntry, checkForEmptySpace);
        textNode->NodeText.SetString(newString);
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

        atkNode->NodeText.SetString(data);

        return text;
    }

    public void ReplaceSeString(ref SeString message, INameDatabaseEntry databaseEntry, bool checkForEmptySpace = true)
    {
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            string curString = tPayload.Text!.ToString();
            tPayload.Text = ReplaceStringPart(curString, databaseEntry, checkForEmptySpace);

            message.Payloads[i] = tPayload;
        }
    }

    public void ReplaceSeString(ref SeString message, string replaceString, INameDatabaseEntry databaseEntry, bool checkForEmptySpace = true)
    {
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            string curString = tPayload.Text!.ToString();
            tPayload.Text = ReplaceStringPart(curString, databaseEntry, checkForEmptySpace);

            message.Payloads[i] = tPayload;
        }
    }

    public string ReplaceStringPart(string baseString, INameDatabaseEntry databaseEntry, bool checkForEmptySpaces = true)
    {
        string baseData = GetBaseName(databaseEntry);
        if (string.IsNullOrEmpty(baseData)) return baseString;

        string? customName = GetCustomName(databaseEntry);
        if (string.IsNullOrEmpty(customName)) return baseString;

        return DoReplacePart(baseString, baseData, customName, checkForEmptySpaces);
    }

    public string DoReplacePart(string baseString, string baseName, string customName, bool checkForEmptySpaces)
    {
        if (baseName.IsNullOrWhitespace() || customName.IsNullOrWhitespace()) return baseString;

        string newBaseString = baseString;
        newBaseString = newBaseString.Replace("[", @"^\[").Replace("]", @"^\]\");

        string regString = baseName;
        if (checkForEmptySpaces) regString = $"\\b" + regString + "\\b";

        newBaseString = Regex.Replace(newBaseString, regString, MakeString(PluginConstants.forbiddenCharacter, 1), RegexOptions.IgnoreCase);
        baseString = newBaseString.Replace(MakeString(PluginConstants.forbiddenCharacter, 1), customName);
        return baseString;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string GetBaseName(INameDatabaseEntry entry) => entry.Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string? GetCustomName(INameDatabaseEntry entry) => entry.ActiveEntry.GetName();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string? GetOldCustomName(INameDatabaseEntry entry) => entry.ActiveEntry.GetOldName();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string MakeString(char c, int count) => new string(c, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
