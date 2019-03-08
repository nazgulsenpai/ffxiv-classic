﻿using System;
using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send
{
    class _0x02Packet
    {
        public const ushort OPCODE = 0x0002;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(uint playerActorId, int val)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Seek(8, SeekOrigin.Begin);
                    binWriter.Write((UInt32)playerActorId);
                }
            }

            return new SubPacket(OPCODE, playerActorId, data);
        }
    }
}
