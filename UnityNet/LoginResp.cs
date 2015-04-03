using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Message;

namespace UnityNet
{
    public class LoginResp : ResponseArg
    {
        private string account;
        private string password;

        public void DeserializeTo(Common.Serializa.DataInput input)
        {
            this.account = input.ReadUTF();
            this.password = input.ReadUTF();
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
