using System;

namespace Serv
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            CreatMysql mysql = new CreatMysql();//实例化创建数据库类
            mysql.CreatDatabase();//创建数据库
            mysql.CreatTable();//创建表
            RoomMgr roomMgr = new RoomMgr();
            DataMgr dataMgr = new DataMgr();
            ServNet servNet = new ServNet();
            servNet.proto = new ProtocolBytes();
            servNet.Start("127.0.0.1", 1234);
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        servNet.Close();
                        return;
                    case "print":
                        servNet.Print();
                        break;
                }
            }
        }
	}
}
