﻿using FFXIVClassic.Common;
using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class PongPacket
    {
        public const ushort OPCODE = 0x0001;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket BuildPacket(uint playerActorID, uint pingTicks)
        {          
            byte[] data = new byte[PACKET_SIZE-0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using(BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)pingTicks);
                    binWriter.Write((UInt32)0x14D);
                }
            }

            SubPacket subpacket = new SubPacket(OPCODE, playerActorID, data);
            return subpacket;
        }

    }
}
