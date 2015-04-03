using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;

namespace UnityNet.Event
{
    public abstract class MyEvent : IEvent
    {
        private string type;
        private bool success = true;
        private int errorCode;
        private string errorDes;
        private object parame;
        private int sender;

        public MyEvent()
        {

        }

        public MyEvent(bool success, object parame)
        {
            this.success = success;
            this.parame = parame;
        }

        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        public string Type
        {
            set { this.type = value; }
            get { return this.type; }
        }

        public bool Success
        {
            get { return success; }
            set { this.success = value; }
        }

        public string ErrorDes
        {
            get { return errorDes; }
            set { this.errorDes = value; }
        }

        public object Parame
        {
            set { this.parame = value; }
            get { return parame; }
        }

        public T GetParame<T>()
        {
            return (T)parame;
        }

        public int Sender
        {
            get { return sender; }
            set { sender = value; }
        }
    }
}
