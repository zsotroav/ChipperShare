using System;
using System.IO;

// Chipper library by zsotroav
// Copyright 2021 zsotroav. All rights reserved. Source code available under the AGPL.

namespace LibChipper
{
    internal class External
    {
        public static readonly string WritePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string MainPath = Path.Combine(WritePath, "zsotroav", "chipper");

        public static void SaveBin(string loc, byte[] data)
        {
            using var fs = File.Create(loc);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        public static byte[] LoadBin(string loc)
        {
            return File.ReadAllBytes(loc);
        }
    }
}
