﻿using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class IssueListResponsePacket
    {
        public const ushort OPCODE = 0x01D2;
        public const uint PACKET_SIZE = 0x160;

        public static SubPacket BuildPacket(uint playerActorID, string[] issueStrings)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < (issueStrings.Length <= 5 ? issueStrings.Length : 5); i++)
                    {
                        binWriter.Seek(0x40 * i, SeekOrigin.Begin);
                        binWriter.Write(Encoding.ASCII.GetBytes(issueStrings[i]), 0, Encoding.ASCII.GetByteCount(issueStrings[i]) >= 0x40 ? 0x40 : Encoding.ASCII.GetByteCount(issueStrings[i]));
                    }
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
