using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityNet.Event;
using UnityNet.FSM;
using UnityNet;
using UnityNet.Logging;
using UnityNet.Core.Message;

namespace UnityNet
{
    class Program
    {

        static UnityClient client = null;

        static void Main(string[] args)
        {
            client = new UnityClient(false,"PC");
            Thread processThread = new Thread(ProcessEvents);
            processThread.Start();
            client.ReconnectionSeconds(10);
            client.AddEventListener("1", new EventListenerDelegate<MyEvent>(Onlogin1));
            client.AddEventListener(LoggerEvent.ONLOGGER, new EventListenerDelegate<MyEvent>(OnLogger));
            client.AddEventListener(UnityClientEvent.CONNECTION,new EventListenerDelegate<MyEvent>(OnConnect));
            client.AddEventListener(UnityClientEvent.HANDSHAKE, new EventListenerDelegate<MyEvent>(OnHandSnake));
            client.AddEventListener(UnityClientEvent.CONNECTION_LOST, new EventListenerDelegate<MyEvent>(OnConnectionLost));
            client.AddEventListener(UnityClientEvent.CONNECTION_RESUME, new EventListenerDelegate<MyEvent>(OnConnectResum));
            client.AddEventListener(UnityClientEvent.CHANNEL_DATA_ERROR, new EventListenerDelegate<MyEvent>(OnChannelDataError));
            client.Connect("172.19.60.134", 8601);
            //client.Close();
            Thread.Sleep(43324234);
        }


        private static void ProcessEvents()
        {
            while (true)
            {
                if (client != null)
                {
                    client.ProcessEvents();
                }
            }
        }

        [NetCallBackParame(typeof(LoginResp))]
        static void Onlogin1(MyEvent evt)
        {
            if (evt.Success)
            {
                LoginResp loginResp = evt.GetParame<LoginResp>();
                Console.WriteLine("Account =>> " + loginResp.Account + ",Password =>> "+ loginResp.Password);
            }
        }

        static void OnLogger(MyEvent args)
        {
            LoggerEvent evt = args as LoggerEvent;
            string message = string.Concat(new object[] { "[UnityNet - ", LoggerEvent.LogEventType(evt.LogLevel), "] ", evt.ErrorDes });
            Console.WriteLine(message);
        }

        static void OnConnect(MyEvent arg)
        {
            UnityClientEvent evt = arg as UnityClientEvent;
            client.Logger.Info("OnConnect =>> success:" + evt.Success + ",parame :" + evt.Parame);
           
            Thread thread = new Thread(sendLoginReq);
            thread.Start();
        }

        private static void sendLoginReq()
        {
            while (true)
            {

                if (client.IsConnected)
                {
                    LoginReq req = new LoginReq();
                    req.Account = "test1";
                    req.Password = "test2";
                    client.Request("1", req);
                }
                Thread.Sleep(100);
            }
        }

        static void OnConnectionLost(MyEvent arg)
        {
            UnityClientEvent evt = arg as UnityClientEvent;
            Console.WriteLine("OnConnectionLost =>> success:" + evt.Success + ",parame :" + evt.Parame);
            client.Logger.Info("OnConnectionLost =>> success:" + evt.Success + ",parame :" + evt.Parame);
        }

        static void OnHandSnake(MyEvent arg)
        {
            UnityClientEvent evt = arg as UnityClientEvent;
            HandSnakeResp resp = evt.GetParame<HandSnakeResp>();

            client.Logger.Info("OnHandSnake =>> success:" + evt.Success + ",parame :" + resp);
        }

        static void OnConnectResum(MyEvent arg)
        {
            UnityClientEvent evt = arg as UnityClientEvent;
            HandSnakeResp resp = evt.GetParame<HandSnakeResp>();

            Console.WriteLine("OnConnectResum:" + resp.GetType().FullName);
        }

        static void OnChannelDataError(MyEvent arg)
        {
            UnityClientEvent evt = arg as UnityClientEvent;
            HandSnakeResp resp = evt.GetParame<HandSnakeResp>();
            Console.WriteLine("OnHandSnake:" + resp.GetType().FullName);
        }
    }
}
