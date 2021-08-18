using System;
using System.Drawing;
using System.IO;

// Chipper library by zsotroav
// Copyright 2021 zsotroav. All rights reserved. Source code available under the AGPL.

namespace LibChipper
{
    internal class External
    {
        public static readonly string WritePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string DocumentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static readonly string ChipperPath = Path.Combine(WritePath, "zsotroav", "chipper");

        public static void SaveBin(string loc, byte[] data)
        {
            if (!FileExists(loc))
            {
                Directory.GetParent(loc)?.Create();
                File.Create(loc).Close();
            }
            using var fs = File.Create(loc);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        public static byte[] LoadBin(string loc) => File.ReadAllBytes(loc);

        public static bool FileExists(string loc) => File.Exists(loc);

        public static string NameFromPath(string path) => Path.GetFileName(path);

        public static string ExtFromPath(string path) => Path.GetExtension(path);
    }
}
