using System;

namespace Serv
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            CreatMysql mysql = new CreatMysql();//ʵ�����������ݿ���
            mysql.CreatDatabase();//�������ݿ�
            mysql.CreatTable();//������
            RoomMgr roomMgr = new RoomMgr();
            DataMgr dataMgr = new DataMgr();
            ServNet servNet = new ServNet();
            servNet.proto = new ProtocolBytes();
            servNet.Start("172.16.220.76", 7788);
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
