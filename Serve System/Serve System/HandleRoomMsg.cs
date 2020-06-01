using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public partial class HandlePlayerMsg
{
	//获取房间列表
	public void MsgGetRoomList(Player player, ProtocolBase protoBase)
	{
        //获取数值
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        string protoName = proto.GetString(start, ref start);
        player.tempData.mapadress = proto.GetString(start, ref start);
        player.Send (RoomMgr.instance.GetRoomList(player));
    }

    //判断用户是否含有已创建的房间
    public void MsgHaveRoom(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HaveRoom");
        if (RoomMgr.instance.HaveRoom(player)) protocol.AddInt(0);
            else protocol.AddInt(-1);
        player.Send(protocol);
        Console.WriteLine("MsgHaveRoom" + player.id);
    }

	//创建房间
	public void MsgCreateRoom(Player player, ProtocolBase protoBase)
	{
        //获取数值
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        string protoName = proto.GetString(start, ref start);
        string RoomName = proto.GetString(start, ref start);
        string RoomAdress = proto.GetString(start, ref start);
        string RoomIns = proto.GetString(start, ref start);
        ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("CreateRoom");
		RoomMgr.instance.CreateRoom (player,RoomName,RoomAdress,RoomIns);
		protocol.AddInt(0);
		player.Send (protocol);
		Console.WriteLine ("MsgCreateRoom Ok " + player.id);
	}

    //获取该用户的所有房间名称
    public void MsgGetRoomNameList(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomNameList");
        List<string> namelist = RoomMgr.instance.GetPlayerRoom(player);
        protocol.AddInt(namelist.Count);
        foreach(string name in namelist)
        {
            protocol.AddString(name);
        }
        player.Send(protocol);
    }
    

    //加入展厅
    public void MsgEnterRoom(Player player, ProtocolBase protoBase)
	{
		//初始化
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		string Name = protocol.GetString (start, ref start); //展厅名称
        Console.WriteLine ("[收到MsgEnterRoom]" + player.id + " " + Name);
		protocol = new ProtocolBytes ();
		protocol.AddString ("EnterRoom");//添加协议名
		//根据展厅名称判断展厅是否存在
		if (!RoomMgr.instance.list.ContainsKey(Name)) 
		{
			Console.WriteLine ("MsgEnterRoom index err " + player.id);
			protocol.AddInt(-1);//展厅不存在，返回加入失败信息
			player.Send (protocol);
			return;
		}
		Room room = RoomMgr.instance.list[Name];//根据展厅名称在展厅字典中获取展厅实体类
		//判断展厅是否满员，若未满员则添加用户
		if (room.AddPlayer (player))
		{
			room.Broadcast(RoomMgr.instance.GetRoomList(player));//广播给展厅中的用户
			protocol.AddInt(0);//添加成功信息
			player.Send (protocol);
		}
		else 
		{
			Console.WriteLine ("MsgEnterRoom maxPlayer err " + player.id);//打印用户ID
			protocol.AddInt(-1);//添加失败信息
			player.Send (protocol);
		}
        //在场景中添加player预制体
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("AddPlayer");
        proto.AddString(player.id);
        proto.AddFloat(player.tempData.posX);//X轴坐标
        proto.AddFloat(player.tempData.posY);// Y轴坐标
        proto.AddFloat(player.tempData.posZ);// Z轴坐标
        proto.AddFloat(player.tempData.rotX);// X轴欧拉角
        proto.AddFloat(player.tempData.rotY);// X轴欧拉角
        proto.AddFloat(player.tempData.rotZ);// X轴欧拉角
        room.Broadcast(proto);//广播给展厅中的所有用户
    }

    //获取房间信息
    public void MsgGetRoomInfo(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string name = protocol.GetString(start, ref start);//房间名称
        player.Send(RoomMgr.instance.GetRoomInfo(name));
	}

    //离开房间
    public void MsgLeaveRoom(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("LeaveRoom");
		
		//条件检测
		if (player.tempData.status != PlayerTempData.Status.Room) 
		{
			Console.WriteLine ("MsgLeaveRoom status err " + player.id);
			protocol.AddInt (-1);
			player.Send (protocol);
			return;
		}
		//处理
		protocol.AddInt (0);
		player.Send (protocol);
		RoomMgr.instance.LeaveRoom (player);
    }

    //删除房间
    public void MsgDeleteRoom(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string roomname = protocol.GetString(start, ref start);//房间名称
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("DeleteRoom");
        if (RoomMgr.instance.DeleteRoom(player, roomname)) proto.AddInt(0);
            else proto.AddInt(-1);
        player.Send(proto);
    }

  
    //同步用户
    public void MsgUpdateUnitInfo(Player player, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        float posX = protocol.GetFloat(start, ref start);
        float posY = protocol.GetFloat(start, ref start);
        float posZ = protocol.GetFloat(start, ref start);
        float rotX = protocol.GetFloat(start, ref start);
        float rotY = protocol.GetFloat(start, ref start);
        float rotZ = protocol.GetFloat(start, ref start);
        //获取房间
        Room room = player.tempData.room;
        player.tempData.posX = posX;
        player.tempData.posY = posY;
        player.tempData.posZ = posZ;
        player.tempData.rotX = rotX;
        player.tempData.rotY = rotY;
        player.tempData.rotZ = rotZ;
        player.tempData.lastUpdateTime = Sys.GetTimeStamp();
        //广播
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("UpdateUnitInfo");
        protocolRet.AddString(player.id);
        protocolRet.AddFloat(posX);
        protocolRet.AddFloat(posY);
        protocolRet.AddFloat(posZ);
        protocolRet.AddFloat(rotX);
        protocolRet.AddFloat(rotY);
        protocolRet.AddFloat(rotZ);
        try
        {
            room.Broadcast(protocolRet);
        }
        catch
        { }
    }
    
}