﻿using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class StartGMTicketPacket
    {
        public const ushort OPCODE = 0x01D3;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, bool startGM)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = (byte)(startGM ? 1 : 0);
            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
