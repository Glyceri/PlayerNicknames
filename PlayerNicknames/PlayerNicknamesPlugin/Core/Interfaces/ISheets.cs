﻿namespace PlayerNicknames.PlayerNicknamesPlugin.Core.Interfaces;

internal interface ISheets
{
    string? GetWorldName(ushort worldID);
    ushort? GetWorldID(string worldname);
}
