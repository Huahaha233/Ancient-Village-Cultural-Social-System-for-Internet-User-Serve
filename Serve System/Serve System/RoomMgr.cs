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
	public void CreateRoom(Player player,string RoomName,string RoomIns)
	{
		Room room = new Room ();
		lock (list) 
		{
            room.Author = player.id;//将创建房间的玩家的ID赋值给房间
            room.Name = RoomName;
            room.Ins = RoomIns;
            player.tempData.rooms.Add(room);
			list.Add(room);
		}
	}
    //删除房间
    public bool DeleteRoom(Player player, string RoomName)
    {
        Room room = new Room();
        foreach(Room rooms in list)
        {
            if (rooms.Name == RoomName)
            {
                room = rooms;
                break;
            }
        }
        if (room != null)
        {
            lock (list)
            {
                list.Remove(room);
                player.tempData.rooms.Remove(room);
                return true;
            }
        }
        else return false;
        
    }
    //玩家离开
    public void LeaveRoom(Player player)
	{
		PlayerTempData tempdata = player.tempData;
		Room room = tempdata.room;
		lock(list)
		{
			room.DelPlayer(player.id);
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
            protocol.AddString(room.Author);
        }
		return protocol;
	}

    //获取房间信息
    public ProtocolBytes GetRoomInfo(int index)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        //房间信息
        protocol.AddString(list[index-1].Ins);//房间的简介，这里需要减1，由于传入的值是从1开始的，而list从0开始
        return protocol;
    }

    //判断当前用户是否已创建房间
    public bool HaveRoom(Player player)
    {
        if (player.tempData.rooms.Count!=0) return true;
        return false;
    }

    public List<Room> GetPlayerTempDataRoom(string id)
    {
        List<Room> rooms = new List<Room>();
        foreach(Room room in list)
        {
            if(room.Author==id)
            {
                rooms.Add(room);
            }
        }
        return rooms;
    }
}