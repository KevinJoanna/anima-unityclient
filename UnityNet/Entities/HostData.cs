using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Entities
{
	public class HostData
	{
        private string gameType;
        private string gameName;
        private string nickname;
        private short connectedPlayers;
        private short playerLimit;
        internal string ip;
        internal int port;
        private string passwordProtected;
        private string comment;

        public string GameType
        {
            get { return gameType; }
            set { gameType = value; }
        }
        
        public string GameName
        {
            get { return gameName; }
            set { gameName = value; }
        }
       
        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }
       
        public short ConnectedPlayers
        {
            get { return connectedPlayers; }
            set { connectedPlayers = value; }
        }
       
        public short PlayerLimit
        {
            get { return playerLimit; }
            set { playerLimit = value; }
        }
       
        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
       

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        

        public string PasswordProtected
        {
            get { return passwordProtected; }
            set { passwordProtected = value; }
        }
       
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public override int GetHashCode()
        {
            return this.ip.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is HostData))
            {
                return false;
            }
            HostData player = (HostData)other;
            return player.ip.Equals(this.ip) && player.port == this.port;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("---------------------\n");
            builder.Append("GameType :" + this.gameType + "\n");
            builder.Append("GameName :" + this.gameName + "\n");
            builder.Append("Nickname :" + this.nickname + "\n");
            builder.Append("ConnectedPlayers :" + this.connectedPlayers + "\n");
            builder.Append("PlayerLimit :" + this.playerLimit + "\n");
            builder.Append("Ip :" + this.ip + "\n");
            builder.Append("port :" + this.port + "\n");
            builder.Append("passwordProtected :" + this.passwordProtected + "\n");
            builder.Append("comment :" + this.comment + "\n");
            return builder.ToString();
        }
	}
}
