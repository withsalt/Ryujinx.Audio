﻿namespace Ryujinx.HLE.Loaders.Elf
{
    public struct ElfSymbol64
    {
#pragma warning disable CS0649 // Field is never assigned to
        public uint NameOffset;
        public byte Info;
        public byte Other;
        public ushort SectionIndex;
        public ulong ValueAddress;
        public ulong Size;
#pragma warning restore CS0649
    }
}
