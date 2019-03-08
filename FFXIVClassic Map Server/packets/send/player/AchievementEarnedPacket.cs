﻿using System;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class AchievementEarnedPacket
    {
        public const ushort OPCODE = 0x019E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, uint achievementID)
        {
            return new SubPacket(OPCODE, sourceActorId, BitConverter.GetBytes((UInt64)achievementID));
        }
    }
}
