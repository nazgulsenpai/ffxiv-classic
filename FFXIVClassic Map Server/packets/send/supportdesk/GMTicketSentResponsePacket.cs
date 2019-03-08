﻿using FFXIVClassic.Common;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class GMTicketSentResponsePacket
    {
        public const ushort OPCODE = 0x01D5;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, bool wasSent)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            data[0] = (byte)(wasSent ? 0x1 : 0x0);

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
