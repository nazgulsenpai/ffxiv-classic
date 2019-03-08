﻿using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetCurrentMountGoobbuePacket
    {

        public const ushort OPCODE = 0x01a0;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, int appearanceId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = (byte)(appearanceId & 0xFF);
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
