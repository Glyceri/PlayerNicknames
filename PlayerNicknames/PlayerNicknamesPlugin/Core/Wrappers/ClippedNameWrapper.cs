using Dalamud.Utility;
using PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;
using PlayerNicknames.PlayerNicknamesPlugin.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Wrappers;

internal class ClippedNameWrapper : IClippedNameDatabase
{
    readonly Dictionary<ulong, IClippedName> clippedNames = new Dictionary<ulong, IClippedName>();
    readonly Dictionary<ulong, (string, IClippedName)> customClippedNames = new Dictionary<ulong, (string, IClippedName)>();

    public IClippedName GetClippedName(INameDatabaseEntry entry)
    {
        if (clippedNames.TryGetValue(entry.ContentID, out IClippedName? value))
        {
            if (value != null)
            {
                return value;
            }
        }

        IClippedName? clippedName = CreateClippedName(entry);
        if (clippedName == null) clippedName = new ClippedName([entry.Name]);

        clippedNames.Add(entry.ContentID, clippedName);

        return clippedName;
    }

    public IClippedName? FromCustomName(INameDatabaseEntry entry)
    {
        if (customClippedNames.TryGetValue(entry.ContentID, out (string, IClippedName) value))
        {
            if (value.Item1 == entry.ActiveEntry.GetOldName()) return value.Item2;
            customClippedNames.Remove(entry.ContentID);
        }

        string? oldName = entry.ActiveEntry.GetOldName();
        if (oldName == null) return new ClippedName(["1"]);

        string[]? clipped = [oldName, ..ClipLine(oldName)];
        if (clipped == null) return new ClippedName(["2"]);

        string[]? sorted = SortArray(clipped);
        if (sorted == null) return new ClippedName(["3"]);

        IClippedName clippedName = new ClippedName(sorted);

        customClippedNames.Add(entry.ContentID, (oldName, clippedName));

        return clippedName;
    }

    IClippedName? CreateClippedName(INameDatabaseEntry entry)
    {
        string[]? nameVariants = NameVariants(entry);
        if (nameVariants == null) return null;

        string[]? clipped = Clip(nameVariants);
        if (clipped == null) return null;

        string[]? sorted = SortArray(clipped);
        if (sorted == null) return null;

        return new ClippedName(clipped);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string[]? SortArray(string[] array)
    {
        try
        {
            array = array.Distinct().ToArray();
            Array.Sort(array, (x, y) => x.Length.CompareTo(y.Length));
            array = array.Reverse().ToArray();
        }
        catch
        {
            return null;
        }

        return array;
    }

    string[]? NameVariants(INameDatabaseEntry entry)
    {
        string[] nameParts = entry.Name.Split(' ');
        if (nameParts.Length != 2) return null;

        string firstName = nameParts[0];
        string lastName = nameParts[1];

        return 
        [
            $"{firstName} {lastName}",
            $"{Initial(firstName)} {lastName}",
            $"{firstName} {Initial(lastName)}",
            $"{Initial(firstName)} {Initial(lastName)}",
        ];
    }

    string[]? Clip(string[] nameVariants)
    {
        if (nameVariants.Length != 4) return null;

        string nameVariant1 = nameVariants[0];
        string nameVariant2 = nameVariants[1];
        string nameVariant3 = nameVariants[2];
        string nameVariant4 = nameVariants[3];

        // what the fuck...
        return
        [
            nameVariant1,
            nameVariant2,
            nameVariant3,
            nameVariant4,
            ..ClipLine(nameVariant1),
            ..ClipLine(nameVariant2),
            ..ClipLine(nameVariant3),
            ..ClipLine(nameVariant4),
        ];
    }

    string[] ClipLine(string line)
    {
        if (line.Length == 0) return [string.Empty];
        List<string> lines = new List<string>();

        for (int i = 0; i < line.Length; i++)
        {
            string newLine = line[..^i]; // sure enough...
            newLine = newLine.Trim();
            if (newLine.IsNullOrWhitespace()) continue;
            newLine += "..."; // Yepp c:
            newLine = newLine.Replace("....", "...");
            lines.Add(newLine);
        }

        return lines.ToArray();
    }

    string Initial(string namePart)
    {
        // wtf ????
        if (namePart.Length < 1) return namePart;

        string newName = namePart.ToUpperInvariant();

        return $"{newName[0]}.";
    }
}

internal readonly struct ClippedName : IClippedName
{
    readonly int _length;
    public int Length => _length;
    readonly string[] ClippedNames;
    public string this[int index] { get => ClippedNames[index]; }

    public ClippedName(string[] clippedNames)
    {
        ClippedNames = clippedNames;
        _length = clippedNames.Length;
    }
}