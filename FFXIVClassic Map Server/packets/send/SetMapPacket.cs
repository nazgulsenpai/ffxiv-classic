﻿using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send
{
    class SetMapPacket
    {
        public const ushort OPCODE = 0x0005;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(uint playerActorID, uint mapID, uint regionID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((uint)mapID);
                    binWriter.Write((uint)regionID);
                    binWriter.Write((uint)0x28);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
