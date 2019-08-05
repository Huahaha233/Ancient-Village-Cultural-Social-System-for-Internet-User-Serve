using System;

public partial class HandleConnMsg
{
	//心跳
	//协议参数：无
	public void MsgHeatBeat(Conn conn, ProtocolBase protoBase)
	{
		conn.lastTickTime = Sys.GetTimeStamp();
		Console.WriteLine("[更新心跳时间]" + conn.GetAdress());
	}

	//注册
	//协议参数：str用户名,str密码
	//返回协议：-1表示失败 0表示成功
	public void MsgRegister(Conn conn, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		string id = protocol.GetString (start, ref start);
		string pw = protocol.GetString (start, ref start);
        string sex = protocol.GetString(start, ref start);
        string adr = protocol.GetString(start, ref start);
        string que = protocol.GetString(start, ref start);
        string ans = protocol.GetString(start, ref start);
        string phone = protocol.GetString(start, ref start);
        string strFormat = "[收到注册协议]" + conn.GetAdress();
		Console.WriteLine ( strFormat  + " 用户名：" + id + " 密码：" + pw + " 性别：" + sex + " 住址：" + adr + " 密保问题：" + que + " 密保答案：" + phone+" 联系方式：" + phone);
		//构建返回协议
		protocol = new ProtocolBytes ();
		protocol.AddString ("Register");
		//注册
		if(DataMgr.instance.Register (id, pw,sex,adr,que,ans,phone))
		{
			protocol.AddInt(0);
		}
		else
		{
			protocol.AddInt(-1);
		}
		//创建角色
		DataMgr.instance.CreatePlayer (id);
		//返回协议给客户端
		conn.Send (protocol);
	}

	//登录
	//协议参数：str用户名,str密码
	//返回协议：-1表示失败 0表示成功
	public void MsgLogin(Conn conn, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		string id = protocol.GetString (start, ref start);
		string pw = protocol.GetString (start, ref start);
		string strFormat = "[收到登录协议]" + conn.GetAdress();
		Console.WriteLine (strFormat  + " 用户名：" + id + " 密码：" + pw);
		//构建返回协议
		ProtocolBytes protocolRet = new ProtocolBytes ();
		protocolRet.AddString ("Login");
		//验证
		if (!DataMgr.instance.CheckPassWord (id, pw)) 
		{
            protocolRet.AddInt(-1);
			conn.Send (protocolRet);
			return;
		}
		//是否已经登录
		ProtocolBytes protocolLogout = new ProtocolBytes ();
		protocolLogout.AddString ("Logout");
		if (!Player.KickOff (id, protocolLogout)) 
		{
			protocolRet.AddInt(-1);
			conn.Send (protocolRet);
			return;
		}

        //获取玩家数据
        PlayerData playerData = DataMgr.instance.GetPlayerData(id);
        if (playerData == null)
        {
            protocolRet.AddInt(-1);
            conn.Send(protocolRet);
            return;
        }
        conn.player = new Player(id, conn);
        conn.player.data = playerData;
        //事件触发
        ServNet.instance.handlePlayerEvent.OnLogin(conn.player);
        //返回
        protocolRet.AddInt(0);
		conn.Send (protocolRet);
		return;
	}

	//下线
	//协议参数：
	//返回协议：0-正常下线
	public void MsgLogout(Conn conn, ProtocolBase protoBase)
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("Logout");
		protocol.AddInt (0);
		if (conn.player == null) 
		{
			conn.Send (protocol);
			conn.Close ();
		}
		else 
		{
			conn.Send (protocol);
			conn.player.Logout();
		}
	}
    
    //忘记密码
    public void MsgSendForget(Conn conn, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start, ref start);
        string strFormat = "[收到忘记密码协议]" + conn.GetAdress();
        Console.WriteLine(strFormat + " 用户名：" + id);
        //构建返回协议
        protocol = new ProtocolBytes();
        protocol.AddString("SendForget");
        //注册
        if (DataMgr.instance.Forget(id)!="false")
        {
            protocol.AddString(DataMgr.instance.Forget(id));//密保问题
        }
        else
        {
            protocol.AddString("false");
        }
        //返回协议给客户端
        conn.Send(protocol);
    }

    //重置密码
    public void MsgReset(Conn conn, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string pw = protocol.GetString(start, ref start);
        string strFormat = "[收到重置密码协议]" + conn.GetAdress();
        Console.WriteLine(strFormat + " 密码：" + pw);
        //构建返回协议
        protocol = new ProtocolBytes();
        protocol.AddString("Reset");
        //注册
        if (DataMgr.instance.Reset(pw))
        {
            protocol.AddInt(0);
        }
        else
        {
            protocol.AddInt(-1);
        }
        //返回协议给客户端
        conn.Send(protocol);
    }
}