using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace Serve_System
{
    class Serve
    {
        //异步程序
        //监听嵌套字
        public Socket listenfd;
        //客户端连接
        public Conn[] conns;
        //最大连接数
        public int maxConn = 50;
        //协议
        public ProtocolBase proto;
        //获取连接池索引，返回负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isuse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        //开启服务器
        public void Start(string host, int port)
        {
            //连接池
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            //socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipadr = IPAddress.Parse(host);
            IPEndPoint ipep = new IPEndPoint(ipadr, port);
            listenfd.Bind(ipep);
            //listen
            listenfd.Listen(maxConn);
            //accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("服务器启动成功！");
            Console.ReadKey();
        }
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();
                if (index < 0)
                {
                    socket.Close();
                    Console.WriteLine("连接人数已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine(adr + "已连接" + "ID:" + index);
                    conn.socket.BeginReceive(conn.readbuff, conn.buffcount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
            try
            {
                int count = conn.socket.EndReceive(ar);
                //关闭信号
                if (count <= 0)
                {
                        Console.WriteLine(conn.GetAdress() + "断开连接");
                        conn.Close();
                        return;
                }
                    conn.buffcount += count;
                    //数据处理
                    ProcessData(conn);
                    conn.socket.BeginReceive(conn.readbuff, conn.buffcount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            }
            catch (Exception e)
            {
                //Console.WriteLine(conn.GetAdress() + "断开连接");
                conn.Close();
            }

            }
        }

        private void ProcessData(Conn conn)
        {
            //小于长度字节
            if (conn.buffcount < sizeof(Int32))
            {
                return;
            }
            //消息长度
            Array.Copy(conn.readbuff, conn.lenBytes, sizeof(Int32));//将readbuff的前4个字节复制到lenbytes中
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);//获取消息的长度
            if (conn.buffcount < conn.msgLength + sizeof(Int32))
            {
                return;
            }
            //处理消息
            ProtocolBase protocol = proto.Decode(conn.readbuff, sizeof(Int32), conn.msgLength);
            //清除已处理的消息
            int count = conn.buffcount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.readbuff, sizeof(Int32) + conn.msgLength, conn.readbuff, 0, count);//将readbuff的中msgLength长度后面的内容复制到readbuff起始位置
            conn.buffcount = count;
            if (conn.buffcount > 0)
            {
                ProcessData(conn);
            }
        }

        //广播
        public void Broadcast(ProtocolBase protocol)
        {
            for (int i = 0; i < conns.Length; i++)
            {
                if (!conns[i].isuse)
                    continue;
                //if (conns[i].player == null)
                //    continue;
                Send(conns[i], protocol);
            }
        }

        //发送
        public void Send(Conn conn, ProtocolBase protocol)
        {
            byte[] bytes = protocol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            try
            {
                conn.socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送消息]" + conn.GetAdress() + " : " + e.Message);
            }
        }

        

        //打印信息
        public void Print()
        {
            Console.WriteLine("===服务器登录信息===");
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isuse)
                    continue;

                string str = "连接[" + conns[i].GetAdress() + "] ";
                //if (conns[i].player != null)
                //    str += "玩家id " + conns[i].player.id;

                Console.WriteLine(str);
            }
        }
    }
}
