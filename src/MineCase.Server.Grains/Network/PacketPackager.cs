﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IO;
using MineCase.Protocol;

namespace MineCase.Server.Network
{
    internal class PacketPackager : IPacketPackager
    {
        private readonly RecyclableMemoryStreamManager _memoryStreamMgr;

        public PacketPackager(RecyclableMemoryStreamManager memoryStreamMgr)
        {
            _memoryStreamMgr = memoryStreamMgr;
        }

        public Task<(uint PacketId, byte[] Data)> PreparePacket(IPacket packet)
        {
            using (var stream = _memoryStreamMgr.GetStream())
            {
                using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
                    packet.Serialize(bw);
                return Task.FromResult((GetPacketId(packet), stream.ToArray()));
            }
        }

        private uint GetPacketId(IPacket packet)
        {
            var typeInfo = packet.GetType().GetTypeInfo();
            var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
            return attr.PacketId;
        }
    }
}
