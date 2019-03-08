﻿using System;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class FriendlistAddedPacket
    {
        public const ushort OPCODE = 0x01CC;
        public const uint PACKET_SIZE = 0x067;

        public static SubPacket BuildPacket(uint sourceActorId, bool isSuccess, long id, bool isOnline, string nameToAdd)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)id);
                    binWriter.Write((byte)(isOnline ? 1 : 0));
                    binWriter.Write((byte)(isSuccess ? 1 : 0));
                    binWriter.Write(Encoding.ASCII.GetBytes(nameToAdd), 0, Encoding.ASCII.GetByteCount(nameToAdd) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(nameToAdd));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
