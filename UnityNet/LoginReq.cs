using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;
using UnityNet.Core.Message;
using UnityNet.Event;

namespace UnityNet
{
    public class LoginReq : RequestArg
    {
        private string account;
        private string password;

        public void SerializaTo(Dataoutput output)
        {
            output.WriteUTF(account);
            output.WriteUTF(password);
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Account
        {
            get { return account; }
            set { account = value; }
        }
    }
}
