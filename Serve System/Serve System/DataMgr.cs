using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataMgr
{
	MySqlConnection sqlConn;
    private string SendForgetID = "";//用户在选择忘记密码按钮时，单独发送ID以此来获取密保问题与密保答案
	//单例模式
	public static DataMgr instance;
	public DataMgr()
	{
		instance = this;
		Connect();
	}
	
	//连接
	public void Connect()
	{
		//数据库
		string connStr = "Database=village;Data Source=127.0.0.1;";
		connStr += "User Id=root;Password=123456789;port=3306";
		sqlConn = new MySqlConnection(connStr);
		try
		{
			sqlConn.Open();
		}
		catch (Exception e)
		{
			Console.Write("[DataMgr]Connect " + e.Message);
			return;
		}
	}

	//判定安全字符串
	public bool IsSafeStr(string str)
	{
		return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
	}

	//是否存在该用户
	private bool CanRegister(string id)
	{
		//防sql注入
		if (!IsSafeStr(id))
			return false;
		//遍历用户信息表，查询id是否存在
		string cmdStr = string.Format("select * from user where UserID='{0}';", id);  
		MySqlCommand cmd = new MySqlCommand (cmdStr, sqlConn);  
		try 
		{
			MySqlDataReader dataReader = cmd.ExecuteReader (); 
			bool hasRows = dataReader.HasRows;
			dataReader.Close();
			return !hasRows;
		}
		catch(Exception e)
		{
			Console.WriteLine("[DataMgr]CanRegister fail " + e.Message);
			return false;
		}
	}
	
	//注册
	public bool Register(string id, string pw, string sex, string adr,string que,string ans,string phone)
	{
		//防sql注入
		if (!IsSafeStr (id) || !IsSafeStr (pw)) 
		{
			Console.WriteLine("[DataMgr]Register 使用非法字符");//提示
			return false;
		}
		//判断是否该用户是否已存在
		if (!CanRegister(id)) 
		{
			Console.WriteLine("[DataMgr]Register !CanRegister");//提示
			return false;
		}
		//将用户注册信息写入数据库User表
		string cmdStr = string.Format("insert into user set UserID ='{0}' ,UserPSW ='{1}',Sex='{2}',Adress='{3}',Question='{4}',Answer='{5}',Phone='{6}';",
                                       id, pw,sex,adr,que,ans,phone);
        //操作数据库
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		try
		{
			cmd.ExecuteNonQuery();
			return true;
		}
		catch(Exception e)
		{
			Console.WriteLine("[DataMgr]Register " + e.Message);
			return false;
		}
	}

    //忘记密码
    public string Forget(string id)
    {
        string que="";//密保问题
        string ans="";//密保答案
        SendForgetID = id;//赋值，以便将ID传入重置密码协议当中
        //防sql注入
        if (!IsSafeStr(id))
        {
            Console.WriteLine("[DataMgr]Register 使用非法字符");
            return "false";
        }
        //能否存在该用户
        if (CanRegister(id))
        {
            Console.WriteLine("[DataMgr]Register !CanRegister");
            return "false";
        }
        //读取数据库User表
        string cmdStr = string.Format("select * from user where UserID='{0}';", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                que = dataReader.GetString(4);
                ans = dataReader.GetString(5);
            }
            dataReader.Close();
            Console.WriteLine(que + ";" + ans);
            return que+";"+ans;
        }
        catch (Exception e)
        {
            Console.WriteLine("[DataMgr]Register " + e.Message);
            return "false";
        }
    }

    //重置密码
    public bool Reset(string pw)
    {
        //防sql注入
        if (!IsSafeStr(pw))
        {
            Console.WriteLine("[DataMgr]Register 使用非法字符");
            return false;
        }
        string formatStr = "update user set UserPSW =@data where UserID = '{0}';";
        string cmdStr = string.Format(formatStr, SendForgetID);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        cmd.Parameters.AddWithValue("@data", pw);
        //将修改后的密码存入数据库User表
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[DataMgr]Register " + e.Message);
            return false;
        }
    }

    //创建角色
    public bool CreatePlayer(string id)
	{
		//防sql注入
		if (!IsSafeStr(id))
			return false;
		//序列化
		IFormatter formatter = new BinaryFormatter ();
		MemoryStream stream = new MemoryStream ();
		PlayerData playerData = new PlayerData ();
		try 
		{
			formatter.Serialize(stream, playerData);
		} 
		catch(Exception e) 
		{
			Console.WriteLine("[DataMgr]CreatePlayer 序列化 " + e.Message);
			return false;
		}
		byte[] byteArr = stream.ToArray();
		//写入数据库
		string cmdStr = string.Format ("insert into player set UserID ='{0}' ,data =@data;",id);
		MySqlCommand cmd = new MySqlCommand (cmdStr, sqlConn);
        cmd.Parameters.AddWithValue("@data", byteArr);
        try 
		{
			cmd.ExecuteNonQuery ();
			return true;
		} 
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]CreatePlayer 写入 " + e.Message);
			return false;
		}
	}

	//检测用户名密码
	public bool CheckPassWord(string id, string pw)
	{
		//防sql注入
		if (!IsSafeStr (id)||!IsSafeStr (pw))
			return false;
		//根据用户名与密码检验用户身份
		string cmdStr = string.Format("select * from user where UserID='{0}' and UserPSW='{1}';", id, pw);  
		MySqlCommand cmd = new MySqlCommand (cmdStr, sqlConn);  
		try 
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			bool hasRows = dataReader.HasRows;//用户身份是否正确
			dataReader.Close();
			return hasRows;//返回结果
		}
		catch(Exception e)
		{
			Console.WriteLine("[DataMgr]CheckPassWord " + e.Message);
			return false;
		}
	}

	//获取玩家数据
	public PlayerData GetPlayerData(string id)
	{
		PlayerData playerData = null;
		//防sql注入
		if (!IsSafeStr(id))
			return playerData;
		//查询
		string cmdStr = string.Format("select * from player where UserID ='{0}';", id);
		MySqlCommand cmd = new MySqlCommand (cmdStr, sqlConn); 
		byte[] buffer;
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader(); 
			if(!dataReader.HasRows)
			{
				dataReader.Close();
				return playerData;
			}
			dataReader.Read();
			
			long len = dataReader.GetBytes(1, 0, null, 0, 0);//1是data  
			buffer = new byte[len];  
			dataReader.GetBytes(1, 0, buffer, 0, (int)len);
			dataReader.Close();
		}
		catch(Exception e)
		{
			Console.WriteLine("[DataMgr]GetPlayerData 查询 " + e.Message);
			return playerData;
		}
        //反序列化
        MemoryStream stream = new MemoryStream(buffer);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            playerData = (PlayerData)formatter.Deserialize(stream);
            return playerData;
        }
        catch (SerializationException e)
        {
            Console.WriteLine("[DataMgr]GetPlayerData 反序列化 " + e.Message);
            return playerData;
        }
    }


	//保存角色
	public bool SavePlayer(Player player)
	{
		string id = player.id;
		PlayerData playerData = player.data;
		//序列化
		IFormatter formatter = new BinaryFormatter ();
		MemoryStream stream = new MemoryStream ();
		try 
		{
			formatter.Serialize(stream, playerData);
		} 
		catch(Exception e) 
		{
			Console.WriteLine("[DataMgr]SavePlayer 序列化 " + e.Message);
			return false;
		}
		byte[] byteArr = stream.ToArray();
        //写入数据库
        string cmdStr = string.Format("update player set data =@data where id = '{0}';", id);
		MySqlCommand cmd = new MySqlCommand (cmdStr, sqlConn);
        cmd.Parameters.AddWithValue("@data", byteArr);
        try
		{
			cmd.ExecuteNonQuery ();
			return true;
		} 
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]CreatePlayer 写入 " + e.Message);
			return false;
		}
	}

}