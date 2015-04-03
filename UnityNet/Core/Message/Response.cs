using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Message
{
    public class Response : AbstractMessage
    {
        /**
          * OK
         */
        public const int OK = 200;
        /**
         * Bad Request
         */
        public const int BAD = 404;
        /**
         * Service not found.
         */
        public const int SERVICE_NOT_FOUND = 500;
        /**
         * Service error.
         */
        public const int SERVICE_ERROR = 600;

        private int sequence;

        private int errorCode = OK;

        private string errorDes;

        public Response(int id)
            : base(id, AbstractMessage.TYPE_RESPONSE)
        {

        }

        public Response(int id, int errorCode)
            : this(id)
        {
            this.errorCode = errorCode;
        }

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        public string ErrorDes
        {
            get { return errorDes; }
            set { errorDes = value; }
        }
    }
}
