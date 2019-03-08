﻿using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryBeginChangePacket
    {
        public const ushort OPCODE = 0x016D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, uint targetActorId)
        {
            byte[] data = new byte[8];
            data[0] = 2;
            return new SubPacket(OPCODE, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint playerActorID)
        {
            byte[] data = new byte[8];
            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
