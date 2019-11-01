using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serve_System
{
    interface IHandleMysql
    {
        Dictionary<string, string> selectMySQL(string handlestr,string name);//name为条件
        void insertMySQL(string handlestr);
        void deleteMySQL(string handlestr);
    }
    //对数据库进行怎删改查操作
    public class HandleMysql:IHandleMysql
    {
        public string connectStr= "server=localhost;user id=root;password=123456789;database=village;SslMode = none";
        
        public Dictionary<string,string> selectMySQL(string handlestr,string name)
        {
            Dictionary<string, string> chat = new Dictionary<string, string>();
            //select查询操作
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");

                string sql = handlestr; //从表userinfo查询所有行数据
                //string sql = "select * from userinfo"; //从表userinfo查询所有行数据
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                //三种不同的执行sql语句方式, 在合适的场合选择使用
                //cmd.ExecuteReader();//执行一些查询
                //cmd.ExecuteNonQuery();//插入 删除
                //cmd.ExecuteScalar();//执行一些查询 返回单个的值
                
                MySqlDataReader reader = cmd.ExecuteReader();// reader 中保存查询到的数据 对应表中一行行数据保存
                //遍历输出所有查询到的数据
                //reader.Read()在读取下一行数据，如果读取成功，返回true, 如果没有下一行数据，读取失败，返回值为 false
                while (reader.Read()) //利用reader.Read() 返回值 这个特性在while循环中遍历所有读取到数据
                {
                    if(reader[1].ToString()==name) chat.Add(reader[2].ToString() , reader[3].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return chat;
        }


        public void insertMySQL(string handlestr)
        {
            //Insert插入操作
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");
                //string sql = "insert into userinfo(uname, upwd) values('kkk', '234')"; //插入数据到表
                string sql = handlestr; //插入数据到表
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                int result = cmd.ExecuteNonQuery();//返回值是数据库中受影响的数据的行数
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        
        public void updateMySQL()
        {
            //update修改操作
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");

                string sql = "update userinfo set uname='fff', upwd='789' where id = 3"; //从表userinfo查询所有行数据
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                int result = cmd.ExecuteNonQuery();//返回值是数据库中受影响的数据的行数
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        public void deleteMySQL(string handlestr)
        {
            //delete删除操作
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");
                string sql = handlestr; //从表userinfo删除id为3的一行数据
                //string sql = "delete from userinfo where id = 3"; //从表userinfo删除id为3的一行数据
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                int result = cmd.ExecuteNonQuery();//返回值是数据库中受影响的数据的行数
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        //执行一些查询 返回单个的值 使用ExecuteScalar()函数
        public void readUsersCount()
        {
            //delete删除操作
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");

                string sql = "select count(*) from userinfo"; //从表userinfo中获取数据 个数
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                /*
                 * 得到数据个数
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int count = Convert.ToInt32(reader[0].ToString());
                Console.WriteLine(count);
                */

                //和上面的执行结果一样 得到数据个数
                object o = cmd.ExecuteScalar();
                int count = Convert.ToInt32(o.ToString());
                Console.WriteLine(count);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        public void readDateOtherMethod()
        {
            //查询取得数据的其他方式
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");

                string sql = "select * from userinfo"; //从表userinfo中获取数据 个数
                MySqlCommand cmd = new MySqlCommand(sql, conn);


                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //普通方式
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());

                    //根据索引取得数据
                    //Console.WriteLine(reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2));

                    //根据列名取得数据
                    Console.WriteLine(reader.GetInt32("id") + " " + reader.GetString("uname") + " " + reader.GetString("upwd"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        //根据用户名和密码查询数据库中是否存在
        public bool selectDateForRule(string username, string password)
        {
            MySqlConnection conn = new MySqlConnection(connectStr); //并没有去跟数据库建立链接
            try
            {
                conn.Open();
                Console.WriteLine("已经建立连接");

                //我们自己按照查询条件去组拼sql
                //string sql = "select * from userinfo where uname = '" + username + "' and upwd = '" +  password + "'"; 

                //其他的组拼sql方式
                string sql = "select * from userinfo where uname = @username and upwd = @password ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);


                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                conn.Close();
            }
            return false;

        }
    }
}
