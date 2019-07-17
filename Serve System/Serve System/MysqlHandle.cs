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
    }
}
