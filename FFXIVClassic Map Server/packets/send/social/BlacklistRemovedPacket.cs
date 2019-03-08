﻿using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class BlacklistRemovedPacket
    {
        public const ushort OPCODE = 0x01CA;
        public const uint PACKET_SIZE = 0x048;

        public static SubPacket BuildPacket(uint sourceActorId, bool isSuccess, string nameToRemove)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((byte)(isSuccess ? 1 : 0));
                    binWriter.Write(Encoding.ASCII.GetBytes(nameToRemove), 0, Encoding.ASCII.GetByteCount(nameToRemove) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(nameToRemove));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
