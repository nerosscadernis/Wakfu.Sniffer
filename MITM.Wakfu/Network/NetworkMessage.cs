using Rebirth.Common.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MITM.Wakfu.Network
{
    public class NetworkMessage
    {
        public short Lenght { get; set; }
        public short Id { get; set; }
        public byte Seq { get; set; }
        public byte[] Datas { get; set; }
        public byte[] CompleteDatas { get; set; }

        public void Read(IDataReader reader, bool isClient)
        {
            var writer = new BigEndianWriter();
            Lenght = reader.ReadShort();
            writer.WriteShort(Lenght);
            if (isClient)
               writer.WriteByte(reader.ReadByte());
            Id = reader.ReadShort();
            writer.WriteShort(Id);
            if(Id != 1036)
            {
                Datas = reader.ReadBytes(Lenght - (isClient ? 5 : 4));
                writer.WriteBytes(Datas);
                CompleteDatas = writer.Data;
            }
        }
    }
}
