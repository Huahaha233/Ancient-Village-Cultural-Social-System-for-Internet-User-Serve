using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Net.Sockets;

namespace Serve_System
{   //此类是异步socket程序
    class Conn
    {
        public const int BUFFER_SIZE = 1024 * 1024;//常量
        public Socket socket;
        public bool isuse = false;//是否使用
        public byte[] readbuff = new byte[BUFFER_SIZE];
        public int buffcount = 0;
        //沾包分包
        public byte[] lenBytes = new byte[sizeof(UInt32)];
        //消息的长度
        public Int32 msgLength = 0;
        //构造函数
        public Conn()
        {
            readbuff = new byte[BUFFER_SIZE];
        }
        //初始化
        public void Init(Socket socket)
        {
            this.socket = socket;
            isuse = true;
            buffcount = 0;

        }
        //缓冲区剩余字节数
        public int BuffRemain()
        {
            return BUFFER_SIZE - buffcount;
        }
        //获取客户端地址
        public string GetAdress()
        {
            if (!isuse)
                return "无法获取地址";
            return socket.RemoteEndPoint.ToString();
        }
        public void Close()
        {
            if (!isuse)
                return;
            //Console.WriteLine(GetAdress()+"断开连接");
            
            socket.Close();
            isuse = false;
        }

        public void SaveInMysql(string str)
        {

            string connetStr = "server=127.0.0.1;port=3306;user=root;password=123456;database=chatroom;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
            MySqlConnection conn = new MySqlConnection(connetStr);
            MySqlCommand command = null;
            try
            {
                conn.Open();
                command = conn.CreateCommand();
                command.CommandText = "INSERT INTO user_chat(chat_id,user_name,chat_name,chatcontent,imagedata,chat_time) VALUES(@chat_id,@user_name,@chat_name,@chatcontent,@imagedata,@chat_time)";
                command.Parameters.AddWithValue("@chat_id", null);
                string nowtime = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "-" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                command.Parameters.AddWithValue("@chat_time", nowtime);
                command.ExecuteNonQuery();
            }
            catch
            {
                //Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }


        }

        //在数据库中读取数据
        public string ReadChatContent()
        {

            return "";
        }
    }
}
