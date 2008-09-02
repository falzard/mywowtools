﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoWReader;
using UpdateFields;

namespace UpdatePacketParser {
	public class WowCorePacketReader : PacketReaderBase {
		private GenericReader _reader;

		public WowCorePacketReader(string filename) {
			_reader = new GenericReader(filename, Encoding.ASCII);
			_reader.ReadBytes(3);                    // PKT
			_reader.ReadBytes(2);                    // 0x02, 0x02
			_reader.ReadByte();                      // 0x06
			ushort build = _reader.ReadUInt16();     // build
			_reader.ReadBytes(4);                    // client locale
			_reader.ReadBytes(20);                   // packet key
			_reader.ReadBytes(64);                   // realm name

			UpdateFieldsLoader.LoadUpdateFields(build);
		}

		public override Packet ReadPacket() {
			if(_reader.PeekChar() < 0) {
				return null;
			}
			var direction = _reader.ReadByte();
			var unixtime = _reader.ReadUInt32();
			var tickcount = _reader.ReadUInt32();
			var size = _reader.ReadInt32();

			var packet = new Packet();
			packet.Size = size;
			packet.Code = direction == 0xFF ? _reader.ReadInt16() : _reader.ReadInt32();
			packet.Data = _reader.ReadBytes(size - (direction == 0xFF ? 2 : 4));
			return packet;
		}
	}
}