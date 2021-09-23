using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using The_Project.Accounts;
using The_Project.Extensions;

#nullable enable
namespace The_Project.Networking
{
    public class Listener
    {

        public readonly TcpListener Server;
        public int Port { get; }

        public Listener(UserId UserId)
        {
            Port = GeneratePort(UserId.MinPort, UserId.MaxPort);
            Server = new(IPAddress.Parse("127.0.0.1"), Port);
            Server.Start();
        }

        private int GeneratePort(int Min, int Max)
        {
            return new Random().Next(Min, Max);
        }

        public RecipientConnection ListenAndConnect(string AccountId)
        {
            // buffer
            byte[] bytesBuffer = new byte[256];
            string? accountIdBuffer;
            RecipientConnection? Connection = null;

            while (Connection is null)
            {
                // if no pending connection, continue loop
                if (!Server.Pending()) continue;
                // accept pending connection
                TcpClient Client = Server.AcceptTcpClient();

                // check account id
                NetworkStream Stream = Client.GetStream();
                while (Stream.Read(bytesBuffer, 0, bytesBuffer.Length) != 0)
                {
                    accountIdBuffer = Encoding.UTF8.GetString(bytesBuffer);
                    // verify account id
                    if (accountIdBuffer == AccountId)
                    {
                        Stream.Write(new BitArray(new bool[8] { true, true, true, true, true, true, true, true}).ToByteArray());
                        Connection = new(Client);
                    } else
                    {
                        Stream.Write(new byte[1]);
                        Client.Close();
                    }
                }
                Server.Stop();
            }
            return Connection;
        }
    }
}
