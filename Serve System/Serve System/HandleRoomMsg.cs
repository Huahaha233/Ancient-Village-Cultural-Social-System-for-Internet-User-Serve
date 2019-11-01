using System;
using System.Collections.Generic;

public partial class HandlePlayerMsg
{
	//获取房间列表
	public void MsgGetRoomList(Player player, ProtocolBase protoBase)
	{
		player.Send (RoomMgr.instance.GetRoomList());
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
        string RoomIns = proto.GetString(start, ref start);
        ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("CreateRoom");
		RoomMgr.instance.CreateRoom (player,RoomName,RoomIns);
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

    //获取用户选择房间的全部资源信息
    public void MsgGetResoureList(Player player, ProtocolBase protoBase)
    {
        //先获得房间的名称
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protocol.GetString(start,ref start);
        string RoomName = protocol.GetString(start,ref start);
        ProtocolBytes protoRet = new ProtocolBytes();
        protoRet.AddString("GetResoureList");
        protoRet.AddString(RoomName);
        if (!RoomMgr.instance.list.ContainsKey(RoomName))
        {
            protoRet.AddInt(-1);
            player.Send(protoRet);
            return;
        }
        Room room = RoomMgr.instance.list[RoomName];
        protoRet.AddInt(room.resouredata.Count);
        foreach(string str in room.resouredata.Keys)
        {
            protoRet.AddString(str);
            protoRet.AddString(room.resouredata[str].resouresort);
        }
        player.Send(protoRet);
        Console.WriteLine("MsgGetResoureList Ok " + player.id);
    }

    //加入房间
    public void MsgEnterRoom(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		string Name = protocol.GetString (start, ref start); 
        Console.WriteLine ("[收到MsgEnterRoom]" + player.id + " " + Name);
		protocol = new ProtocolBytes ();
		protocol.AddString ("EnterRoom");
		//判断房间是否存在
		if (!RoomMgr.instance.list.ContainsKey(Name)) 
		{
			Console.WriteLine ("MsgEnterRoom index err " + player.id);
			protocol.AddInt(-1);
			player.Send (protocol);
			return;
		}
		Room room = RoomMgr.instance.list[Name];
		//添加玩家
		if (room.AddPlayer (player))
		{
			room.Broadcast(RoomMgr.instance.GetRoomList());
			protocol.AddInt(0);
			player.Send (protocol);
		}
		else 
		{
			Console.WriteLine ("MsgEnterRoom maxPlayer err " + player.id);
			protocol.AddInt(-1);
			player.Send (protocol);
		}
        //在场景中添加player预制体
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("AddPlayer");
        proto.AddString(player.id);
        proto.AddFloat(player.tempData.posX);
        proto.AddFloat(player.tempData.posY);
        proto.AddFloat(player.tempData.posZ);
        proto.AddFloat(player.tempData.rotX);
        proto.AddFloat(player.tempData.rotY);
        proto.AddFloat(player.tempData.rotZ);
        room.Broadcast(proto);
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

    //增加资源
    //协议参数：
    public void MsgAddResoure(Player player, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string RoomName = protocol.GetString(start, ref start);
        string ResoureName = protocol.GetString(start, ref start);
        string ResoureIns = protocol.GetString(start, ref start);
        string ResoureSort = protocol.GetString(start, ref start);
        string ResoureAdress = protocol.GetString(start, ref start);

        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("AddResoure");
        //判断是否存在该房间
        if (!RoomMgr.instance.list.ContainsKey(RoomName))
        {
            protocolRet.AddInt(-1);
            player.Send(protocolRet);
            return;
        }
        //判断资源名称是否存重复
        if (RoomMgr.instance.list[RoomName].resouredata.ContainsKey(ResoureName))
        {
            protocolRet.AddInt(-1);
            player.Send(protocolRet);
            return;
        }
        Room room = RoomMgr.instance.list[RoomName];
        Resoure resoure = new Resoure();
        resoure.resourename = ResoureName;
        resoure.resoureins = ResoureIns;
        resoure.resouresort = ResoureSort;
        resoure.resoureadress = ResoureAdress;
        room.resouredata.Add(ResoureName,resoure);
        RoomMgr.instance.ReFlashPlayData(player, ResoureSort, 1);
        //处理
        protocolRet.AddInt(0);
        player.Send(protocolRet);
        Console.WriteLine("MsgAddResoure " + player.id);
    }

    //删除资源
    public void MsgDeleteResoure(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string roomname = protocol.GetString(start, ref start);//房间名称
        string resourename = protocol.GetString(start, ref start);//资源名称
        string sort = protocol.GetString(start, ref start);//类型名称
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("DeleteResoure");
        proto.AddString(roomname);
        proto.AddString(resourename);
        proto.AddString(RoomMgr.instance.list[roomname].resouredata[resourename].resoureadress);//存储地址
        if (RoomMgr.instance.list[roomname].DeleteResoure(player, resourename, sort))
        {
            proto.AddInt(0);
        }
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
        room.Broadcast(protocolRet);
    }

    //增加房间留言
    public void MsgAddRoomChat(Player player, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string ChatMessage = protocol.GetString(start, ref start);
        //获取房间
        Room room = player.tempData.room;
        room.AddChatMessgae(player.id, ChatMessage);//将记录增添到数据库
        //广播
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("AddRoomChat");
        protocolRet.AddString(player.id);
        protocolRet.AddString(ChatMessage);
        room.Broadcast(protocolRet);
    }    
    //获取房间所有留言
    public void MsgGetRoomChat(Player player, ProtocolBase protoBase)
    {
        //获取房间
        Room room = player.tempData.room;
        Dictionary<string,string> chat= room.GetChatMessgae();//从数据库获取房间所有留言
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("GetRoomChat");
        protocolRet.AddInt(chat.Count);
        foreach (string key in chat.Keys)
        {
            protocolRet.AddString(key);
            protocolRet.AddString(chat[key]);
        }
        player.Send(protocolRet);
    }
}