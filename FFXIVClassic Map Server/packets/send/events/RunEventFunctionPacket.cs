﻿using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.events
{
    class RunEventFunctionPacket
    {
        public const ushort OPCODE = 0x0130;
        public const uint PACKET_SIZE = 0x2B8;

        public static SubPacket BuildPacket(uint sourcePlayerActorId, uint eventOwnerActorID, string eventStarter, string callFunction, List<LuaParam> luaParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x80;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourcePlayerActorId);
                    binWriter.Write((UInt32)eventOwnerActorID);
                    binWriter.Write((Byte)5);
                    binWriter.Write(Encoding.ASCII.GetBytes(eventStarter), 0, Encoding.ASCII.GetByteCount(eventStarter) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(eventStarter));
                    binWriter.Seek(0x29, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(callFunction), 0, Encoding.ASCII.GetByteCount(callFunction) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(callFunction));
                    binWriter.Seek(0x49, SeekOrigin.Begin);

                    LuaUtils.WriteLuaParams(binWriter, luaParams);
                }
            }

            return new SubPacket(OPCODE, sourcePlayerActorId, data);
        }
    }
}
