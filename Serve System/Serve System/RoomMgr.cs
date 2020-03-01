using System;
using System.Collections.Generic;
using System.Linq;
using Serve_System;

public class RoomMgr
{
	//单例
	public static RoomMgr instance;
	public RoomMgr()
	{
		instance = this;
	}
	
	//房间列表
    public Dictionary<string, Room> list = new Dictionary<string, Room>();
	//创建房间
	public void CreateRoom(Player player,string RoomName,string RoomAdress,string RoomIns)
	{
		Room room = new Room ();
		lock (list) 
		{
            room.Author = player.id;//将创建房间的玩家的ID赋值给房间
            room.Name = RoomName;
            room.Adress = RoomAdress;
            room.Ins = RoomIns;
            try
            {
                list.Add(RoomName,room);
            }
            catch { }
		}
	}
    //删除房间
    public bool DeleteRoom(Player player, string RoomName)
    {
        if (list.Keys.Contains(RoomName))
        {
            lock (list)
            {
                list.Remove(RoomName);
                IHandleMysql handleMysql = new HandleMysql();
                handleMysql.deleteMySQL("delete from roomchat where RoomName = "+RoomName);
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
            try
            {
                room.DelPlayer(player.id);
            }
            catch { }
            player.tempData.room = null;
		}
        //广播
        if (room != null)
        {
            //在场景中删除player预制体
            ProtocolBytes proto = new ProtocolBytes();
            proto.AddString("DelPlayer");
            proto.AddString(player.id);
            room.Broadcast(proto);
        }
    }

	//列表
	public ProtocolBytes GetRoomList(Player player)
	{
        ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomList");
		int count = list.Count;
		//房间数量
		protocol.AddInt (count);
		//每个房间信息
		foreach(Room room in list.Values)
		{
            if (player.tempData.mapadress == room.Adress)
            {
                protocol.AddString(room.Name);
                protocol.AddInt(room.list.Count);//房间中的人数
                protocol.AddString(room.Author);
            }
        }
		return protocol;
	}
    //获取房间信息
    public ProtocolBytes GetRoomInfo(string name)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        //房间信息
        protocol.AddString(list[name].Ins);//房间的简介，这里需要减1，由于传入的值是从1开始的，而list从0开始
        return protocol;
    }

    //判断当前用户是否已创建房间
    public bool HaveRoom(Player player)
    {
        foreach(Room room in list.Values)
        {
            if(room.Author==player.id)return true;
        }
        return false;
    }
    //获取当前用户所拥有的房间名称
    public List<string> GetPlayerRoom(Player player)
    {
        List<string> namelist = new List<string>();
        foreach(string name in list.Keys)
        {
            if (list[name].Author == player.id) namelist.Add(name);
        }
        return namelist;
    }
    //刷新用户的基本资料
    public void ReFlashPlayData(Player player,string sort,int index)
    {
        switch (sort)
        {
            case "picture":
                player.data.picturecount += index;
                break;
            case "video":
                player.data.videocount += index;
                break;
            case "mdoel":
                player.data.modelcount += index;
                break;
        }
    }
}