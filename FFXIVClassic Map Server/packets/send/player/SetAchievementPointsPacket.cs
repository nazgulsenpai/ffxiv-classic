﻿using System;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetAchievementPointsPacket
    {
        public const ushort OPCODE = 0x019C;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, uint numAchievementPoints)
        {            
            return new SubPacket(OPCODE, sourceActorId, BitConverter.GetBytes((UInt64) numAchievementPoints));
        }
    }
}
