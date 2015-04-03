using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Handler;
using UnityNet.Core.Message;
using UnityNet.Core.Remoting;
using UnityNet.Core;
using UnityNet.Event;

namespace UnityNet.Core.Handler
{
    public class DefaultHandler :AbstractHandler
    {

        public DefaultHandler(ExchangeClient client)
            : base(client)
        {
             
        }

        public override void OnReceive(object message)
        {
            if (message is AbstractMessage)
            {
               ExchangeClient client = endpoint as ExchangeClient;

               if (message is Response)
               {
                   Response response = message as Response;
                   UnityClientEvent evt = new UnityClientEvent(Convert.ToString(response.Id), response.Content);
                   evt.Success = (response.ErrorCode == Response.OK);
                   evt.ErrorCode = response.ErrorCode;
                   evt.ErrorDes = response.ErrorDes;
                   client.Parent.Dispatch(evt);
               }else if (message is Push)
               {
                   Push push = message as Push;
                   UnityClientEvent evt = new UnityClientEvent(Convert.ToString(push.Id), push.Content);
                   evt.Sender = push.Identity;
                   client.Parent.Dispatch(evt);
               }
               else if (message is BroadCast)
               {
                   BroadCast broadcast = message as BroadCast;
                   UnityClientEvent evt = new UnityClientEvent(Convert.ToString(broadcast.Id), broadcast.Content);
                   evt.Sender = broadcast.Identity;
                   client.Parent.Dispatch(evt);
               }
            }
            else
            {
                if (message is HandSnakeResp)
                {
                    HandlerHandSnakeResp((HandSnakeResp)message);
                }
                else if (message is HeartBeatResp)
                {
                    HandlerHeatBeatResp((HeartBeatResp)message);
                }else if (message is KickClient)
                {
                    HandlerKickClient((KickClient)message);
                }
            }
        }

        private void HandlerKickClient(KickClient kickClient)
        {
            ExchangeClient client = endpoint as ExchangeClient;
            client.Kick(kickClient.Reason);
        }

        private void HandlerHandSnakeResp(HandSnakeResp resp)
        {
            ExchangeClient client = endpoint as ExchangeClient;
            client.HandlerHandSnake(resp);
        }

        private void HandlerHeatBeatResp(HeartBeatResp resp)
        {
            if (resp.OneWay)
            {
                ExchangeClient client = endpoint as ExchangeClient;
                client.Request(new HeartBeatReq(true));
            }
        }
    }
}
