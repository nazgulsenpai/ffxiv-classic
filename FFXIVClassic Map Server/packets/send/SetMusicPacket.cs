﻿using System;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send
{
    class SetMusicPacket
    {
        public const ushort OPCODE = 0x000C;
        public const uint PACKET_SIZE = 0x28;

        public const ushort EFFECT_IMMEDIATE           = 0x1;
        public const ushort EFFECT_CROSSFADE           = 0x2; //??
        public const ushort EFFECT_LAYER               = 0x3; //??
        public const ushort EFFECT_FADEIN              = 0x4;
        public const ushort EFFECT_PLAY_NORMAL_CHANNEL = 0x5; //Only works for multi channeled music
        public const ushort EFFECT_PLAY_BATTLE_CHANNEL = 0x6;

        public static SubPacket BuildPacket(uint sourceActorId, ushort musicID, ushort musicTrackMode)
        {
            ulong combined = (ulong)(musicID | (musicTrackMode << 16));
            return new SubPacket(OPCODE, 0, BitConverter.GetBytes(combined));
        }
    }
}
