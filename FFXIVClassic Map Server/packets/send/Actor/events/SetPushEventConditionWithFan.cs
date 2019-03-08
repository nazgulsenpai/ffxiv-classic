﻿using FFXIVClassic_Map_Server.actors;
using System;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.events
{
    class SetPushEventConditionWithFan
    {
        public const ushort OPCODE = 0x0170;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket BuildPacket(uint sourceActorId, EventList.PushFanEventCondition condition)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)condition.radius);
                    binWriter.Write((UInt32)0xbfc90fdb);
                    binWriter.Write((UInt32)0x3f860a92);
                    binWriter.Write((UInt32)sourceActorId); //Actor Id
                    binWriter.Write((Single)10.0f);
                    binWriter.Seek(4, SeekOrigin.Current);
                    binWriter.Write((Byte)(condition.outwards ? 0x11 : 0x1)); //If == 0x10, Inverted Bounding Box
                    binWriter.Write((Byte)0);
                    binWriter.Write((Byte)(condition.silent ? 0x1 : 0x0)); //Silent Trigger
                    binWriter.Write(Encoding.ASCII.GetBytes(condition.conditionName), 0, Encoding.ASCII.GetByteCount(condition.conditionName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(condition.conditionName));
                }
            }
            new SubPacket(OPCODE, sourceActorId, data).DebugPrintSubPacket();
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
