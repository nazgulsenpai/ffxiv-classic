﻿using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorQuestGraphicPacket
    {
        public const int NONE            = 0x0;
        public const int QUEST           = 0x2;
        public const int NOGRAPHIC       = 0x3;
        public const int QUEST_IMPORTANT = 0x4;

        public const ushort OPCODE = 0x00E3;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, int iconCode)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)iconCode);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
