using System;
using MySql.Data.MySqlClient;

namespace Serve_System
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatDatabase();
            CreatTable();
            Serve server = new Serve();
            server.Start("127.0.0.1", 1234);
        }

        //创建数据库
        private static void CreatDatabase()//创建数据库
        {
            IMysqlHandle handle = new MysqlHandle();
            handle.CreatDatabase("CREATE DATABASE IF NOT EXISTS chatroom DEFAULT CHARSET utf8 COLLATE utf8_general_ci;");
        }

        private static void CreatTable()//创建数据库表
        {
            IMysqlHandle handle = new MysqlHandle();
            handle.CreatTable("CREATE TABLE IF NOT EXISTS `user` (`user_id` INT UNSIGNED AUTO_INCREMENT,  `user_name` CHAR(10) NOT NULL, `user_psw` CHAR(10) NOT NULL,PRIMARY KEY(`user_id`)) DEFAULT CHARSET = utf8");
        }

    }
}
