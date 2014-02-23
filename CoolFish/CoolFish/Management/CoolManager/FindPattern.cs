// Copyright 2009 Apoc @ ApocDev.com | Apoc @ MMOwned.com | Apoc @ GameDeception.net
//#define X64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using CoolFishNS.Utilities;

namespace CoolFishNS.Management.CoolManager
{
    /// <summary>
    ///     Pattern finding class for matching specific bytes to find memory addresses
    ///     Credits to Dominik, Patrick, Bobbysing, and whoever else I forgot, for most of the ripped code here!
    /// </summary>
    public class FindPattern
    {
        private int _numberToFind;

        /// <summary>
        ///     Lets us know if we found all the patterns we looked for in the patterns file we loaded
        /// </summary>
        public int NotFoundCount
        {
            get { return _numberToFind - _patterns.Count; }
        }

#if !X64
        public readonly Dictionary<string, uint> _patterns = new Dictionary<string, uint>();
#else
        public readonly Dictionary<string, ulong> _patterns = new Dictionary<string, ulong>();
#endif

        /// <summary>
        ///     Creates a new instance of the <see cref="FindPattern" /> class. This class will read from a specified patterns XML
        ///     file
        ///     and search out those patterns in the specified process's memory.
        /// </summary>
        /// <param name="patternFile">The full path to the pattern XML file.</param>
        /// <param name="processHandle">An open process handle to the process to read memory from.</param>
        /// <param name="startAddress">The 'base' address of the process (or module)</param>
        /// <param name="endAddress">The 'end' of the process (or module). Eg; where to stop reading memory from.</param>
#if !X64
        public FindPattern(string patternFile, IntPtr processHandle, uint startAddress, uint endAddress)
#else
        public FindPattern(string patternFile, IntPtr processHandle, ulong startAddress, ulong endAddress)
#endif
        {
            // Get a temporary set of data to work with. :)
            byte[] data = ReadBytes(processHandle, (IntPtr) startAddress, (int) (endAddress - startAddress));
            LoadFile(XElement.Load(patternFile), data, startAddress);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="FindPattern" /> class. This class will read from a specified patterns XML
        ///     file
        ///     and search out those patterns in the specified process's memory.
        /// </summary>
        /// <param name="stream">The input stream to read from instead of a filename</param>
        /// <param name="processHandle">An open process handle to the process to read memory from.</param>
        /// <param name="startAddress">The 'base' address of the process (or module)</param>
        /// <param name="endAddress">The 'end' of the process (or module). Eg; where to stop reading memory from.</param>
#if !X64
        public FindPattern(Stream stream, IntPtr processHandle, uint startAddress, uint endAddress)
#else
        public FindPattern(Stream stream, IntPtr processHandle, ulong startAddress, ulong endAddress)
#endif
        {
            // Get a temporary set of data to work with. :)
            byte[] data = ReadBytes(processHandle, (IntPtr) startAddress, (int) (endAddress - startAddress));
            LoadFile(XElement.Load(stream), data, startAddress);
        }




#if !X64
        public FindPattern(Stream stream, Process process)
#else
        public FindPattern(Stream stream, IntPtr processHandle, ulong startAddress, ulong endAddress)
#endif
        {
            var basObject = process.MainModule;
            var baseAddress = basObject.BaseAddress;
            // Get a temporary set of data to work with. :)
            byte[] data = ReadBytes(process.Handle, baseAddress , basObject.ModuleMemorySize );
            LoadFile(XElement.Load(stream), data, (uint)baseAddress);
        }

        /// <summary>
        ///     Retrieves an address from the found patterns stash.
        /// </summary>
        /// <param name="name">The name of the pattern, as per the XML file provided in the constructor of this class instance.</param>
        /// <returns></returns>
        public IntPtr this[string name]
        {
            get { return Get(name); }
        }


        private static byte[] ReadBytes(IntPtr processHandle, IntPtr address, int count)
        {
            var ret = new byte[count];
            int numRead;
            if (NativeMethods.ReadProcessMemory(processHandle, address, ret, count, out numRead) && numRead == count)
            {
                return ret;
            }
            Logging.Log("Error Code: " + Marshal.GetLastWin32Error());
            return null;
        }

        /// <summary>
        ///     Retrieves an address from the found patterns stash.
        /// </summary>
        /// <param name="name">The name of the pattern, as per the XML file provided in the constructor of this class instance.</param>
        /// <returns></returns>
        public IntPtr Get(string name)
        {
            return _patterns.ContainsKey(name) ? new IntPtr(_patterns[name]) : IntPtr.Zero;
        }

#if !X64

        private void LoadFile(XContainer file, byte[] data, uint start)
#else
        private void LoadFile(XContainer file, byte[] data, ulong start)
#endif
        {
            // Grab all the <Pattern /> elements from the XML.
            IEnumerable<XElement> pats = from p in file.Descendants("Pattern")
                select p;
            _numberToFind = file.Descendants("Pattern").Count();

            // Each Pattern element needs to be handled seperately.
            // The enumeration we're goinv over, is in document order, so attributes such as 'start'
            // should work perfectly fine.
            foreach (XElement pat in pats)
            {
#if !X64
                uint tmpStart = 0;
#else
                ulong tmpStart = 0;
#endif

                string name = pat.Attribute("desc").Value;
                string mask = pat.Attribute("mask").Value;
                byte[] patternBytes = GetBytesFromPattern(pat.Attribute("pattern").Value);

                // Make sure we're not getting some sort of screwy XML data.
                if (mask.Length != patternBytes.Length)
                    throw new Exception("Pattern and mask lengths do not match!");

                // If we run into a 'start' attribute, we need to remember that we're working from a 0
                // based 'memory pool'. So we just remove the 'start' from the address we found earlier.
                if (pat.Attribute("start") != null)
                {
#if !X64
                    tmpStart = (uint) (Get(pat.Attribute("start").Value) - (int) start + 1).ToInt32();
#else
                     tmpStart = (uint)(Get(pat.Attribute("start").Value) - (int)start + 1).ToInt64();
                    #endif
                }

#if !X64
                uint found = 0;
#else
                ulong found = 0;
#endif
                try
                {
                    found = Find(data, mask, patternBytes, tmpStart);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                // Actually search for the pattern match...

                if (found == 0)
                {
                    continue;
                }

                // Handle specific child elements for the pattern.
                // <Lea> <Rel> <Add> <Sub> etc
                foreach (XElement e in pat.Elements())
                {
                    switch (e.Name.LocalName)
                    {
                        case "Lea":
#if !X64
                            found = BitConverter.ToUInt32(data, (int) found);
#else
                            found = BitConverter.ToUInt64(data, (int)found);
#endif
                            break;

                        case "Rel":
                            uint instructionSize = uint.Parse(e.Attribute("size").Value, NumberStyles.HexNumber);
                            uint operandOffset = uint.Parse(e.Attribute("offset").Value, NumberStyles.HexNumber);
#if !X64
                            found = (BitConverter.ToUInt32(data, (int) found) + found + instructionSize - operandOffset);
#else
                            found = (BitConverter.ToUInt64(data, (int)found) + found + instructionSize - operandOffset);
#endif
                            break;

                        case "Add":
                            found += uint.Parse(e.Attribute("value").Value, NumberStyles.HexNumber);
                            break;

                        case "Sub":
                            found -= uint.Parse(e.Attribute("value").Value, NumberStyles.HexNumber);
                            break;
                    }
                }

                _patterns.Add(name, found + start);
            }
        }

        private static byte[] GetBytesFromPattern(string pattern)
        {
            // Because I'm lazy, and this just makes life easier.
            string[] split = pattern.Split(new[] {'\\', 'x'}, StringSplitOptions.RemoveEmptyEntries);
            var ret = new byte[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                ret[i] = byte.Parse(split[i], NumberStyles.HexNumber);
            }
            return ret;
        }

#if !X64

        private static uint Find(byte[] data, string mask, byte[] byteMask, uint start)
#else
        private static ulong Find(byte[] data, string mask, byte[] byteMask, ulong start)
#endif
        {
            // There *has* to be a better way to do this stuff,
            // but for now, we'll deal with it.
#if !X64
            for (uint i = start; i < data.Length; i++)
#else
            for (ulong i = start; i < (ulong)data.Length; i++)
#endif
            {
                if (DataCompare(data, (int) i, byteMask, mask))
                    return i;
            }
            return 0;
        }

        private static bool DataCompare(byte[] data, int offset, byte[] byteMask, string mask)
        {
            // Only check for 'x' mismatches. As we'll assume anything else is a wildcard.
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] == 'x' && byteMask[i] != data[i + offset])
                {
                    return false;
                }
            }
            return true;
        }
    }
}