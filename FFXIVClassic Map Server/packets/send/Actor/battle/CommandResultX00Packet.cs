﻿using FFXIVClassic.Common;
using System;
using System.IO;

namespace  FFXIVClassic_Map_Server.packets.send.actor.battle
{
    class CommandResultX00Packet
    {
        public const ushort OPCODE = 0x013C;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(uint sourceActorId, uint animationId, ushort commandId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt32)0); //Num actions (always 0 for this)
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)810); //?

                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
