﻿using System;
using System.IO;

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorSubStatePacket
    {
        public const ushort OPCODE = 0x144;
        public const uint PACKET_SIZE = 0x28;

        enum SubStat : int
        {
            Breakage          = 0x00, // (index goes high to low, bitflags)
            Chant             = 0x01, // [Nibbles: left / right hand = value]) (AKA SubStatObject)
            Guard             = 0x02, // [left / right hand = true] 0,1,2,3) ||| High byte also defines how many bools to use as flags for byte 0x4. 
            Waste             = 0x03, // (High Nibble)
            Mode              = 0x04, // ???
            Unknown           = 0x05, // ???
            SubStatMotionPack = 0x06,
            Unknown2          = 0x07,
        }
        public static SubPacket BuildPacket(uint sourceActorId, SubState substate)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                   binWriter.Write((byte)substate.breakage);
                   binWriter.Write((byte)substate.chantId);
                   binWriter.Write((byte)(substate.guard & 0xF));
                   binWriter.Write((byte)(substate.waste));
                   binWriter.Write((byte)(substate.mode));
                   binWriter.Write((byte)0);
                   binWriter.Write((ushort)substate.motionPack);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
