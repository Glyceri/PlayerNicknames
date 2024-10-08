﻿using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.DirtySystem.Interfaces;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PlayerNicknames.PlayerNicknamesPlugin.Database;

internal class NameEntry : INameEntry
{
    string? name;
    string? oldName;

    readonly IDirtyCaller DirtyCaller;

    public NameEntry(IDirtyCaller dirtyCaller, string? name)
    {
        DirtyCaller = dirtyCaller;
        this.name = name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetName()
    {
        return name;
    }

    public void SetName(string? name)
    {
        name = SanitizeName(name);

        if (this.name == name) return;

        this.oldName = this.name;
        this.name = name;
        SetDirty();
    }

    string? SanitizeName(string? name)
    {
        if (name.IsNullOrWhitespace()) return null;

        try
        {
            name = nameRegex.Replace(name, string.Empty);
            name = urlRegex.Replace(name, string.Empty); // Replaces URLS with NOT URls
            name = spaceRegex.Replace(name, string.Empty); // This allows for only 1 space in the string
            name = name.Replace(PluginConstants.forbiddenCharacter.ToString(), string.Empty); // No ^ allowed
            name = name.Replace("?", string.Empty);

            name = name.Trim();
            if (name.IsNullOrWhitespace()) return null;

            if (name.Length > PluginConstants.FFXIV_MAX_PLAYERNAME_SIZE)
            {
                name = name.Substring(0, PluginConstants.FFXIV_MAX_PLAYERNAME_SIZE);
            }
        }
        catch 
        { 
            return null; 
        }

        return name;
    }

    readonly Regex urlRegex = new Regex(@"\b(?:(?:https?|ftp):\/\/)?(?:(?:[a-z0-9\-]+\.)+[a-z]{2,}|localhost)(?::\d{1,5})?(?:\/[^\s]*)?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    readonly Regex spaceRegex = new Regex(@"(?<=\s.*)\s", RegexOptions.Compiled);
    readonly Regex nameRegex = new Regex(@"[^a-zA-Z\s']", RegexOptions.Compiled);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SetDirty()
    {
        DirtyCaller.DirtyName(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetOldName()
    {
        return oldName;
    }
}
