using System;
using System.Collections.Generic;
using System.Linq; 

public class RoomMgr
{
	//单例
	public static RoomMgr instance;
	public RoomMgr()
	{
		instance = this;
	}
	
	//房间列表
	public List<Room> list = new List<Room>();

	//创建房间
	public void CreateRoom(Player player)
	{
		Room room = new Room ();
		lock (list) 
		{
            room.Author = player.id;//将创建房间的玩家的ID赋值给房间
			list.Add(room);
		}
	}

	//玩家离开
	public void LeaveRoom(Player player)
	{
		PlayerTempData tempdata = player.tempData;
		if (player.tempData.status == PlayerTempData.Status.None)
			return;
		
		Room room = tempdata.room;
		
		lock(list)
		{
			room.DelPlayer(player.id);
			//if(room.list.Count == 0)//当房间内的人数为0时，删除房间
			//	list.Remove(room);
		}
	}

	//列表
	public ProtocolBytes GetRoomList()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomList");
		int count = list.Count;
		//房间数量
		protocol.AddInt (count);
		//每个房间信息
		for (int i=0; i<count; i++) 
		{
			Room room = list[i];
            protocol.AddString(room.Name); 
			protocol.AddInt(room.list.Count);//房间中的人数
            protocol.AddString(room.Ins);
            protocol.AddString(room.Author);
        }
		return protocol;
	}

}