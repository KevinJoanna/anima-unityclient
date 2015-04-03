using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Message;
using UnityNet.Logging;
using UnityNet.Common.Serializa;
using UnityNet.Common.Utils;

namespace UnityNet.Core.Protocol
{
	public class DefaultCodec : Codec
	{
        private Endpoint endpoint;
        //private IHandler handler;
        private Logger log;
        // magic code.
        public short MAGIC;
        //High byte
        public byte MAGIC_HIGH;
        //Low byte
        public byte MAGIC_LOW;

        public DefaultCodec(Endpoint endpoint)
        {
            this.endpoint = endpoint;
            this.log = endpoint.Logger;
            unchecked
            {
                MAGIC = (short)0xdabb;
            }
            MAGIC_HIGH = Bytes.Short2bytes(MAGIC)[0];
            MAGIC_LOW = Bytes.Short2bytes(MAGIC)[1];
        }

        public object Decode(Remoting.Channel ch,Packet packet)
        {
            BinaryDataInput input = new BinaryDataInput(packet.Content);
            int type = packet.Header.Type;
            if (type == Packet.FLAG_HANDSNAKE)
            {
                HandSnakeResp resp = new HandSnakeResp();
                resp.DeserializeTo(input);
                return resp;
            }else if (type == Packet.FLAG_HEARTBEAT)
            {
                HeartBeatResp req = new HeartBeatResp();
                req.DeserializeTo(input);
                return req;
            }
            else if (type == Packet.FLAG_KICK)
            {
                KickClient kick = new KickClient();
                kick.DeserializeTo(input);
                return kick;
            }
            else if (type == Packet.FLAG_MESSAGE)
            {
                int mid = input.ReadInt();
                byte mType = input.ReadByte();
                if (mType == AbstractMessage.TYPE_RESPONSE)
                {
                    Response response = new Response(mid);
                    response.Sequence = input.ReadInt();
                    response.ErrorCode = input.ReadInt();
                    response.ErrorDes = input.ReadUTF();
                    if (response.ErrorCode == Response.OK)
                    {
                        ResponseArg respArg = ResponseMappingInfo.Instance.CreateResponseMapping(mid) as ResponseArg;
                        if (respArg == null)
                        {
                            log.Error("Failed to handle response message,Cause : Cloud not found response mapper id :" + mid);
                        }
                        respArg.DeserializeTo(input);
                        response.Content = respArg;
                    }
                    return response;
                }
                else if (mType == AbstractMessage.TYPE_PUSH)
                {
                    Push push = new Push(mid);
                    push.Identity = input.ReadInt();
                    ResponseArg respArg = ResponseMappingInfo.Instance.CreateResponseMapping(mid) as ResponseArg;
                    if (respArg == null)
                    {
                        log.Error("Failed to handle push message,Cause : Cloud not found response mapper id :" + mid);
                    }
                    respArg.DeserializeTo(input);
                    push.Content = respArg;
                    return push;
                }else if (mType == AbstractMessage.TYPE_BROADCAST)
                {
                    BroadCast broadcast = new BroadCast(mid);
                    broadcast.Identity = input.ReadInt();
                    ResponseArg respArg = ResponseMappingInfo.Instance.CreateResponseMapping(mid) as ResponseArg;
                    if (respArg == null)
                    {
                        log.Error("Failed to handle broadcast message,Cause : Cloud not found response mapper id :" + mid);
                    }
                    respArg.DeserializeTo(input);
                    broadcast.Content = respArg;
                    return broadcast;
                } 
            }
            return null;
        }

        public void Encode(Remoting.Channel ch, object message,ByteArray output)
        {
            ByteArray data = new ByteArray();
            Dataoutput binOutput = new BinaryDataOutput(data);

            byte pType = 0, commpressed = 0;

            if (message is HandSankeReq)
            {
                pType = Packet.FLAG_HANDSNAKE;
                commpressed = PacketHeader.UNCOMPRESSED;
                HandSankeReq req = message as HandSankeReq;
                req.SerializaTo(binOutput);
            }
            else if (message is HeartBeatReq)
            {
                pType = Packet.FLAG_HEARTBEAT;
                commpressed = PacketHeader.UNCOMPRESSED;
                HeartBeatReq req = message as HeartBeatReq;
                req.SerializaTo(binOutput);
            }
            else if (message is Request)
            {
                pType = Packet.FLAG_MESSAGE;
                commpressed = PacketHeader.UNCOMPRESSED;
                Request req = message as Request;
                binOutput.WriteInt(req.Id);
                binOutput.WriteByte(req.Type);
                binOutput.WriteInt(req.Sequence);
                binOutput.WriteBool(req.TwoWay);
                RequestArg content = (RequestArg)req.Content;
                content.SerializaTo(binOutput);
            }

            Dataoutput wholeOutput = new BinaryDataOutput(output);
            wholeOutput.WriteByte(MAGIC_HIGH);
            wholeOutput.WriteByte(MAGIC_LOW);
            wholeOutput.WriteByte(pType);
            wholeOutput.WriteByte(commpressed);
            wholeOutput.WriteInt(data.Length);
            output.WriteBytes(data.Bytes);
        }
    }
}
