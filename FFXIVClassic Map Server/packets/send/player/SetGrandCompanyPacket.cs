﻿using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetGrandCompanyPacket
    {        
        public const ushort OPCODE = 0x0194;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, ushort currentAllegiance, ushort rankLimsa, ushort rankGridania, ushort rankUldah)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Byte)currentAllegiance);
                    binWriter.Write((Byte)rankLimsa);
                    binWriter.Write((Byte)rankGridania);
                    binWriter.Write((Byte)rankUldah);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}
