using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serve_System
{   
    //该接口为数据库操作的接口
    interface IMysqlHandle
    {
        void CreatDatabase(string MysqlStatment);//创建数据库
        void CreatTable(string sql);//创建数据库表
    }
    public class MysqlHandle : IMysqlHandle
    {
        public void CreatDatabase(string MysqlStatment)
        {
            string creatdatabase = "Data Source=localhost;Persist Security Info=yes;UserId=root;PWD=123456;SslMode = none";
            MySqlConnection conn = new MySqlConnection(creatdatabase);
            try
            {
                conn.Open();
                MySqlCommand comm = new MySqlCommand(MysqlStatment, conn);
                comm.ExecuteNonQuery();
                Console.WriteLine("数据库建立完成");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public void CreatTable(string sql)
        {
            //下表为用户注册信息，存放用户的用户名密码等信息
            ChangeMysql(sql);
        }

        private static void ChangeMysql(string Mysql_change)//对数据库进行操作，通过其他函数传入的参数进行操作
        {
            string Mysql_str = "server=localhost;user id=root;password=123456;database=chatroom;SslMode = none";
            MySqlConnection myconn = new MySqlConnection(Mysql_str);
            try
            {
                myconn.Open();
                MySqlCommand mycomm = new MySqlCommand(Mysql_change, myconn);
                mycomm.ExecuteNonQuery();
                Console.WriteLine("建表完成");
            }
            catch (MySqlException ex)
            {
            }
            finally
            {
                myconn.Close();
            }
        }

        //增
        public void Add()
        {
            string Mysql_change = string.Format("insert into user set id='{0}',pw='{1}';",0,1);
        }

        //查
        public string Read(string commandtext,int index,int outdex,string ReadText)//commandtext为查找表名的命令语句、index为传入属性的列数、outdex为需要查找的属性的列数、ReadText传入的查找条件
        {
            string connetStr = "server=127.0.0.1;port=3306;user=root;password=123456;database=chatroom;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
            MySqlConnection conn = new MySqlConnection(connetStr);
            conn.Open();
            MySqlCommand command = null;
            MySqlDataReader dataReader = null;
            try
            {
                string str = "";//注意""与null是有区别的，若是return的值为null则会报错。
                command = conn.CreateCommand();
                command.CommandText = commandtext;
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if ((dataReader.GetString(index) == ReadText))
                    {
                        str= dataReader.GetString(outdex);
                    }
                }
                return str;
            }
            catch
            {
                return "";
            }
            finally { conn.Close(); }
        }
    }
}
