﻿using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class _0x02ReceivePacket
    {
        bool invalidPacket = false;
        uint unknown;

        public _0x02ReceivePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        binReader.BaseStream.Seek(0x14, SeekOrigin.Begin);
                        unknown = binReader.ReadUInt32();
                    }
                    catch (Exception)
                    {
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
