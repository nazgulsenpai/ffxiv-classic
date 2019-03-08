﻿using FFXIVClassic_Map_Server.dataobjects;
using System.Collections.Generic;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryListX64Packet
    {
        public const ushort OPCODE = 0x014C;
        public const uint PACKET_SIZE = 0x1C20;

        public static SubPacket BuildPacket(uint playerActorID, List<InventoryItem> items, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (items.Count - listOffset <= 64)
                        max = items.Count - listOffset;
                    else
                        max = 64;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write(items[listOffset].ToPacketBytes());
                        listOffset++;
                    }
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
