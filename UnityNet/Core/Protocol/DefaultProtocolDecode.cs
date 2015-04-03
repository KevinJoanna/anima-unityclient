using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Protocol;
using UnityNet.Common.Utils;
using UnityNet.FSM;
using UnityNet.Logging;
using UnityNet.Core.Remoting;
using UnityNet.Common.Serializa;

namespace UnityNet.Core.Protocol
{
	public class DefaultProtocolDecode : ProtocolDecode 
	{
        private FiniteStateMachine fsm;
        private readonly ByteArray EMPTY_BUFFER = new ByteArray();
        public static readonly int INT_BYTE_SIZE = 4;
        public static readonly int SHORT_BYTE_SIZE = 2;
        private Packet pendingPacket;
        private int skipBytes = 0;
        private Endpoint endpoint;
        private Logger log;
        private Codec codec;
        // magic code.
        public short MAGIC;
        //High byte
        public byte MAGIC_HIGH;
        //Low byte
        public byte MAGIC_LOW;
        /// <summary>
        /// Packet header binary length
        /// </summary>
        public const int PAKCET_BINARY_LENGTH = 8;

        public DefaultProtocolDecode(Endpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            this.endpoint = endpoint;
            unchecked
            {
                MAGIC = (short)0xdabb;
            }
            MAGIC_HIGH = Bytes.Short2bytes(MAGIC)[0];
            MAGIC_LOW = Bytes.Short2bytes(MAGIC)[1];
            codec = new DefaultCodec(endpoint);
			log = endpoint.Logger;
            this.InitStates();
        }

        private void InitStates()
        {
            this.fsm = new FiniteStateMachine();
            this.fsm.AddAllStates(typeof(PacketReadState));
            this.fsm.AddStateTransition(PacketReadState.WAIT_NEW_PACKET, PacketReadState.WAIT_PACKET_HEADER, PacketReadTransition.NewPakcetReceived);
            this.fsm.AddStateTransition(PacketReadState.WAIT_PACKET_HEADER, PacketReadState.WAIT_DATA, PacketReadTransition.PacketHeaderReceived);
            this.fsm.AddStateTransition(PacketReadState.WAIT_PACKET_HEADER, PacketReadState.WAIT_PACKET_HEADER_FRAGMETN, PacketReadTransition.IncompletePacketHeader);
            this.fsm.AddStateTransition(PacketReadState.WAIT_PACKET_HEADER_FRAGMETN, PacketReadState.WAIT_DATA, PacketReadTransition.WholePacketHeaderReceived);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.PacketFinished);
            this.fsm.AddStateTransition(PacketReadState.WAIT_DATA, PacketReadState.INVALID_DATA, PacketReadTransition.InvalidData);
            this.fsm.AddStateTransition(PacketReadState.INVALID_DATA, PacketReadState.WAIT_NEW_PACKET, PacketReadTransition.InvalidDataFinished);
            this.fsm.SetCurrentState(PacketReadState.WAIT_NEW_PACKET);
        }

        private ByteArray HandlePakcetHeader(ByteArray data)
        {
            string[] messages = new string[1];
            object[] objArray1 = new object[] { "Handling Packet Header. Data Length: ", data.Length};
            messages[0] = string.Concat(objArray1);
            this.log.Debug(messages);
            if (data.Length >= PAKCET_BINARY_LENGTH)
            {
                short magic = data.ReadShort();
                if (magic != MAGIC)
                {
                    object[] objArray2 = new object[] { "Unexpected header magic short: ", magic, "\n", HexDumpFormatter.HexDump(data) };
                    throw new ProtocolCodecException(string.Concat(objArray2));
                }
                //packet header type
                byte type = data.ReadByte();
                //packet header compressed
                byte compressed = data.ReadByte();
                //Packet header length
                int length = data.ReadInt();
                PacketHeader header = new PacketHeader(type, compressed, length);
                pendingPacket.Header = header;
                this.fsm.ApplyTransition(PacketReadTransition.PacketHeaderReceived);

                if (data.Length > PAKCET_BINARY_LENGTH)
                {
                    return this.ResizeByteArray(data, PAKCET_BINARY_LENGTH, data.Length - PAKCET_BINARY_LENGTH);
                }

                data = this.EMPTY_BUFFER;
                return data;

            }

            this.fsm.ApplyTransition(PacketReadTransition.IncompletePacketHeader);
            this.pendingPacket.Content.WriteBytes(data.Bytes);
            data = this.EMPTY_BUFFER;
            return data;
        }

        private ByteArray HandlePacketHeaderFragment(ByteArray data)
        {
            string[] messages = new string[] { "Handling Packet Header fragment. Data: " + data.Length };
            this.log.Debug(messages);
            int count = PAKCET_BINARY_LENGTH - this.pendingPacket.Content.Length;
   
            if (data.Length >= count)
            {
                this.pendingPacket.Content.WriteBytes(data.Bytes, 0, count);
                ByteArray array = new ByteArray();
                array.WriteBytes(this.pendingPacket.Content.Bytes, 0, PAKCET_BINARY_LENGTH);
                array.Position = 0;
                //packet header type
                byte type = array.ReadByte();
                //packet header compressed
                byte compressed = array.ReadByte();
                //Packet header length
                int length = array.ReadInt();
                PacketHeader header = new PacketHeader(type, compressed, length);
                this.pendingPacket.Header = header;
                this.pendingPacket.Content = new ByteArray();
                this.fsm.ApplyTransition(PacketReadTransition.WholePacketHeaderReceived);
                if (data.Length > count)
                {
                    data = this.ResizeByteArray(data, count, data.Length - count);
                    return data;
                }
                data = this.EMPTY_BUFFER;
                return data;
            }
            this.pendingPacket.Content.WriteBytes(data.Bytes);
            data = this.EMPTY_BUFFER;
            return data;
        }

        private ByteArray HandleInvalidData(ByteArray data)
        {
            if (this.skipBytes == 0)
            {
                this.fsm.ApplyTransition(PacketReadTransition.InvalidDataFinished);
                return data;
            }
            int pos = Math.Min(data.Length, this.skipBytes);
            data = this.ResizeByteArray(data, pos, data.Length - pos);
            this.skipBytes -= pos;
            return data;
        }

        private ByteArray HandleNewPacket(ByteArray data)
        {
            //instance Packet and debug log
            string[] messages = new string[] { "Handling New Packet of size " + data.Length };
            this.log.Debug(messages);
            pendingPacket = new Packet();
            this.fsm.ApplyTransition(PacketReadTransition.NewPakcetReceived);
            return data;
        }

        private ByteArray HandlePacketData(Channel ch,ByteArray data)
        {
            int count = this.pendingPacket.Header.ExpectedLen - this.pendingPacket.Content.Length;
           
            ByteArray array = new ByteArray(data.Bytes);
            try
            {
                string[] messages = new string[1];
                object[] objArray1 = new object[] { "Handling Data: ", data.Length, ", previous state: ", this.pendingPacket.Content.Length, "/", this.pendingPacket.Header.ExpectedLen };
                messages[0] = string.Concat(objArray1);
                this.log.Debug(messages);
                if (data.Length >= count)
                {
                    this.pendingPacket.Content.WriteBytes(data.Bytes, 0, count);
                    string[] textArray2 = new string[] { "<<< Packet Complete >>>" };
                    this.log.Debug(textArray2);
                    if (this.pendingPacket.Header.Compressed)
                    {
                        //TODO UnCompress 
                    }
                
                    object mesasge = this.codec.Decode(ch, this.pendingPacket);
                    endpoint.Handler.OnReceive(mesasge);
                    this.fsm.ApplyTransition(PacketReadTransition.PacketFinished);

                    if (data.Length > count)
                    {
                        data = this.ResizeByteArray(data, count, data.Length - count);
                        return data;
                    }
                    data = this.EMPTY_BUFFER;
                    return data;
                }
                else
                {
                    this.pendingPacket.Content.WriteBytes(data.Bytes);
                }

                data = this.EMPTY_BUFFER;
            }
            catch (Exception exception)
            {
                string[] textArray3 = new string[] { "Error handling data: " + exception.Message + " " + exception.StackTrace };
                this.log.Error(textArray3);
                this.skipBytes = count;
                this.fsm.ApplyTransition(PacketReadTransition.InvalidData);
                return array;
            }
            return data;
        }

        private ByteArray ResizeByteArray(ByteArray array, int pos, int len)
        {
            byte[] dst = new byte[len];
            Buffer.BlockCopy(array.Bytes, pos, dst, 0, len);
            return new ByteArray(dst);
        }

        public void Decode(Channel ch, ByteArray input)
        {
            if (input.Length == 0)
            {
                throw new ProtocolCodecException("Unexpected empty packet data:no readable bytes available!");
            }

            if (endpoint.LogLevel == Logging.LogLevel.DEBUG)
            {
                if (input.Length > 0x400)
                {
                    string[] messages = new string[] { "Data Read: Size > 1024, dump omitted" };
                    this.log.Info(messages);
                }
                else
                {
                    string[] textArray2 = new string[] { "Data Read: " + HexDumpFormatter.HexDump(input) };
                    this.log.Info(textArray2);
                }
            }

            input.Position = 0;

            while (input.Length > 0)
            {
                if (this.ReadState == PacketReadState.WAIT_NEW_PACKET)
                {
                    input = this.HandleNewPacket(input);
                }
                else
                {
                    if (this.ReadState == PacketReadState.WAIT_PACKET_HEADER)
                    {
                        input = this.HandlePakcetHeader(input);
                        continue;
                    }
                    if (this.ReadState == PacketReadState.WAIT_PACKET_HEADER_FRAGMETN)
                    {
                        input = this.HandlePacketHeaderFragment(input);
                        continue;
                    }
                    if (this.ReadState == PacketReadState.WAIT_DATA)
                    {
                        input = this.HandlePacketData(ch,input);
                        continue;
                    }
                    if (this.ReadState == PacketReadState.INVALID_DATA)
                    {
                        input = this.HandleInvalidData(input);
                    }
                }
            }
        }

        private PacketReadState ReadState
        {
            get
            {
                return (PacketReadState)this.fsm.GetCurrentState();
            }
        }
    }
}
