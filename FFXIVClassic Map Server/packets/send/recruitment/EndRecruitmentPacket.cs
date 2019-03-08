﻿using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.recruitment
{
    class EndRecruitmentPacket
    {
        public const ushort OPCODE = 0x01C4;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = 1;
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
