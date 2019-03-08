﻿using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.group
{
    class GroupHeaderPacket
    {
        public const uint TYPEID_RETAINER = 0x13881;
        public const uint TYPEID_PARTY = 0x2711;
        public const uint TYPEID_LINKSHELL = 0x4E22;

        public const ushort OPCODE = 0x017C;
        public const uint PACKET_SIZE = 0x98;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, ulong sequenceId, Group group)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write list header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)sequenceId);

                    //Write list id
                    binWriter.Write((UInt64)3);
                    binWriter.Write((UInt64)group.groupIndex);
                    binWriter.Write((UInt64)0);
                    binWriter.Write((UInt64)group.groupIndex);

                    //This seems to change depending on what the list is for
                    binWriter.Write((UInt32)group.GetTypeId());
                    binWriter.Seek(0x40, SeekOrigin.Begin);

                    //This is for Linkshell
                    binWriter.Write((UInt32)group.GetGroupLocalizedName());
                    binWriter.Write(Encoding.ASCII.GetBytes(group.GetGroupName()), 0, Encoding.ASCII.GetByteCount(group.GetGroupName()) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(group.GetGroupName()));

                    binWriter.Seek(0x64, SeekOrigin.Begin);

                    binWriter.Write((UInt32)0x6D);
                    binWriter.Write((UInt32)0x6D);
                    binWriter.Write((UInt32)0x6D);
                    binWriter.Write((UInt32)0x6D);

                    binWriter.Write((UInt32)group.GetMemberCount());
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
