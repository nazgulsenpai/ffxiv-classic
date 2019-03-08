﻿using FFXIVClassic.Common;
using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Lobby_Server.packets
{
    class ErrorPacket
    {
        private const ushort OPCODE = 0x02;

        private UInt64 sequence;
        private uint errorCode;
        private uint statusCode;
        private uint textId;
        private string message;

        public ErrorPacket(UInt64 sequence, uint errorCode, uint statusCode, uint textId, string message)
        {
            this.sequence = sequence;
            this.errorCode = errorCode;
            this.statusCode = statusCode;
            this.textId = textId;
            this.message = message;
        }

        public SubPacket BuildPacket()
        {
            MemoryStream memStream = new MemoryStream(0x210);
            BinaryWriter binWriter = new BinaryWriter(memStream);

            binWriter.Write(sequence);
            binWriter.Write(errorCode);
            binWriter.Write(statusCode);
            binWriter.Write(textId);
            binWriter.Write(Encoding.ASCII.GetBytes(message));

            byte[] data = memStream.GetBuffer();
            binWriter.Dispose();
            memStream.Dispose();
            SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, data);
            subpacket.SetTargetId(0xe0006868);
            return subpacket;
        }
    }
}
