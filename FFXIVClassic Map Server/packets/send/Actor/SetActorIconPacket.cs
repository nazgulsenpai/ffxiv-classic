﻿using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorIconPacket
    {
        public const uint DISCONNECTING = 0x00010000;
        public const uint ISGM = 0x00020000;
        public const uint ISAFK = 0x00000100;

        public const ushort OPCODE = 0x0145;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, uint iconCode)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)iconCode);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
