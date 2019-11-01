using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serv
{
    //创建数据库与数据库表
    public class CreatMysql
    {
        //单例
        public static CreatMysql instance;
        public CreatMysql()
        {
            instance = this;
        } 
        //创建数据库
        public void CreatDatabase()//创建数据库
        {
            string creatdatabase = "Data Source=localhost;Persist Security Info=yes;UserId=root;PWD=123456789;SslMode = none";
            string MysqlStatment = "CREATE DATABASE IF NOT EXISTS village DEFAULT CHARSET utf8 COLLATE utf8_general_ci;";
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

        public void CreatTable()//创建数据库表
        {
            //下表为用户注册信息，存放用户的用户名密码等信息
            string sql = "CREATE TABLE IF NOT EXISTS `user` (`UserID` CHAR(10) NOT NULL,  `UserPSW` CHAR(10) NOT NULL, `Sex` CHAR(1) NOT NULL,`Adress` CHAR(50) NOT NULL,`Question` CHAR(50) NOT NULL,`Answer` CHAR(20) NOT NULL,`Phone` CHAR(11) NOT NULL,PRIMARY KEY(`UserID`)) DEFAULT CHARSET = utf8";
            ChangeMysql(sql);
            //下表为play表，存储playdata
            sql = "CREATE TABLE IF NOT EXISTS `player` (`UserID` CHAR(10) NOT NULL,  `data` BLOB ,PRIMARY KEY(`UserID`)) DEFAULT CHARSET = utf8";
            ChangeMysql(sql);
            //下表为chat表，存储房间内的聊天记录
            sql = "CREATE TABLE IF NOT EXISTS `roomchat` (`ChatId` INT UNSIGNED AUTO_INCREMENT,`RoomName` CHAR(10) NOT NULL,`PlayerId` CHAR(10) NOT NULL,`ChatMessage` CHAR(200) NOT NULL ,`ChatTime` CHAR(20) NOT NULL ,PRIMARY KEY(`ChatId`)) DEFAULT CHARSET = utf8";
            ChangeMysql(sql);
        }

        private void ChangeMysql(string Mysql_change)//对数据库进行操作，通过其他函数传入的参数进行操作
        {
            string Mysql_str = "server=localhost;user id=root;password=123456789;database=village;SslMode = none";
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
