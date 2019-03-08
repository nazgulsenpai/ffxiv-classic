﻿using System;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class SendBlacklistPacket
    {
        public const ushort OPCODE = 0x01CB;
        public const uint PACKET_SIZE = 0x686;

        public static SubPacket BuildPacket(uint sourceActorId, string[] blacklistedNames, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)0);
                    int max;

                    if (blacklistedNames.Length - offset <= 0x32)
                        max = blacklistedNames.Length - offset;
                    else
                        max = 0x32;

                    binWriter.Write((UInt32)max);

                    for (int i = 0; i < max; i++ )
                        binWriter.Write(Encoding.ASCII.GetBytes(blacklistedNames[i]), 0, Encoding.ASCII.GetByteCount(blacklistedNames[i]) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(blacklistedNames[i]));

                    offset += max;
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
