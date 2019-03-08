﻿using FFXIVClassic.Common;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class AddActorPacket
    {
        public const ushort OPCODE = 0x00CA;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, byte val)
        {
            byte[] data = new byte[PACKET_SIZE-0x20];
            data[0] = val; //Why?

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}
