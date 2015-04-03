using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Message
{
    public abstract class AbstractMessage
    {
#region  Message Type Const

        /**
        * Request type
        */
        public const byte TYPE_REQUEST = 0x10;
        /**
         * Notify type
         */
        public const byte TYPE_NOTIFY = 0x20;
        /**
         * Response type
         */
        public const byte TYPE_RESPONSE = 0x30;
        /**
         * Push type
         */
        public const byte TYPE_PUSH = 0x40;
        /**
         * Broadcast type
         */
        public const byte TYPE_BROADCAST = 0x50;

 #endregion
        
#region Message Properties

        /**
         * Message id
         */
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        /**
         * Message type 
         */
        private byte type;

        public byte Type
        {
            get { return type; }
        }

        private int identity;

        public int Identity
        {
            get { return identity; }
            set { identity = value; }
        }
        /**
         * Message content
         */
        private object content;

        public object Content
        {
            get { return content; }
            set { content = value; }
        }

#endregion
      
#region Construct

        public AbstractMessage(int id, byte type)
        {
            this.id = id;
            this.type = type;
        }
#endregion

#region Public method

        /// <summary>
        /// Message type transform string value
        /// </summary>
        /// <returns></returns>
        public string typeToString()
        {
            if (type == TYPE_REQUEST)
            {
                return "request";
            }
            else if (type == TYPE_NOTIFY)
            {
                return "notify";
            }
            else if (type == TYPE_PUSH)
            {
                return "push";
            }
            else if (type == TYPE_RESPONSE)
            {
                return "response";
            }
            else if (type == TYPE_BROADCAST)
            {
                return "broadcast";
            }
            return "N/A";
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("-------------- Message Info -----------------\n");
            builder.Append("Id:\t" + this.id + "\n");
            builder.Append("Type:\t" + typeToString() + "\n");
            builder.Append("Content:\t" + this.content != null ? content.ToString() : "N/A" + "\n");
            builder.Append("---------------------------------------------\n");
            return builder.ToString();
        }
#endregion
       
    }
}
