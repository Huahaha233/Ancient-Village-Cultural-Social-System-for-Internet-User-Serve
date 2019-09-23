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

    //获取用户选择房间的全部资源信息
    public void MsgGetResoureList(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetResoureList");
        protocol.AddInt(player.tempData.rooms.Count);
        for(int i=0;i< player.tempData.rooms.Count; i++)
        {
            protocol.AddString(player.tempData.rooms[i].Name);
            protocol.AddInt(player.data.picturecount+player.data.videocount+player.data.modelcount);
            for(int p=0;p< player.data.picturecount; p++)
            {
                protocol.AddString(player.tempData.rooms[i].picture[p].resourename);
                protocol.AddString(player.tempData.rooms[i].picture[p].resouresourt);
            }
            for (int v = 0; v < player.data.picturecount; v++)
            {
                protocol.AddString(player.tempData.rooms[i].video[v].resourename);
                protocol.AddString(player.tempData.rooms[i].video[v].resouresourt);
            }
            for (int m = 0; m < player.data.picturecount; m++)
            {
                protocol.AddString(player.tempData.rooms[i].model[m].resourename);
                protocol.AddString(player.tempData.rooms[i].model[m].resouresourt);
            }
        }
        player.Send(protocol);
        Console.WriteLine("MsgGetResoureList Ok " + player.id);
    }

    //加入房间
    public void MsgEnterRoom(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		int index = protocol.GetInt (start, ref start)-1; //这里需要减1，由于传入的值是从1开始的，而list从0开始
        Console.WriteLine ("[收到MsgEnterRoom]" + player.id + " " + index);
		protocol = new ProtocolBytes ();
		protocol.AddString ("EnterRoom");
		//判断房间是否存在
		if (index < 0 || index >= RoomMgr.instance.list.Count) 
		{
			Console.WriteLine ("MsgEnterRoom index err " + player.id);
			protocol.AddInt(-1);
			player.Send (protocol);
			return;
		}
		Room room = RoomMgr.instance.list[index];

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
	}

    //获取房间信息
    public void MsgGetRoomInfo(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        int index = protocol.GetInt(start, ref start);//房间序号
        player.Send(RoomMgr.instance.GetRoomInfo(index));
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
		Room room = player.tempData.room;
		RoomMgr.instance.LeaveRoom (player);
		//广播
		if(room != null)
			room.Broadcast(RoomMgr.instance.GetRoomList());
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
        //广播
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
        foreach(Room room in player.tempData.rooms)
        {
            if(room.Name==roomname)
            {
                if(room.DeleteResoure(player, resourename, sort))proto.AddInt(0);
                else proto.AddInt(-1);
                player.Send(proto);
            }
        }
        //广播
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
        //float gunRot = protocol.GetFloat(start, ref start);
        //float gunRoll = protocol.GetFloat(start, ref start);
        //获取房间
        if (player.tempData.status != PlayerTempData.Status.Room)
            return;
        Room room = player.tempData.room;
        //作弊校验 略
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
        //protocolRet.AddFloat(gunRot);
        //protocolRet.AddFloat(gunRoll);
        room.Broadcast(protocolRet);
    }
}