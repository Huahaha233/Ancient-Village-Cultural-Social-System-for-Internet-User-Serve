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
	
	//展厅列表
    public Dictionary<string, Room> list = new Dictionary<string, Room>();
	//创建展厅
	public void CreateRoom(Player player,string RoomName,string RoomAdress,string RoomIns)
	{
        //实例化展厅实体类
		Room room = new Room ();
		lock (list) 
		{
            room.Author = player.id;//将创建展厅的用户ID赋值给展厅作者
            room.Name = RoomName;//展厅名称
            room.Adress = RoomAdress;//展厅地址
            room.Ins = RoomIns;//展厅简介
            try
            {
                list.Add(RoomName,room);//再展厅字典中添加新建的展厅实体类类
            }
            catch { }
		}
	}
    //删除展厅
    public bool DeleteRoom(Player player, string RoomName)
    {
        //根据展厅名称判断是否存在该展厅
        if (list.Keys.Contains(RoomName))
        {
            lock (list)
            {
                list.Remove(RoomName);//从展厅字典中移除展厅
                IHandleMysql handleMysql = new HandleMysql();//调用接口，操纵数据库删除该展厅所有信息
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

	//获取展厅列表
	public ProtocolBytes GetRoomList(Player player)
	{
        ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomList");
		int count = 0;//初始化展厅数量
       //展厅临时列表
        Dictionary<string, Room> templist = new Dictionary<string, Room>();
		//遍历字典添加每个展厅信息
		foreach(Room room in list.Values)
		{
            //判断是否为用户当前选择的展厅
            if (player.tempData.mapadress == room.Adress)
            {
                count++;
                templist.Add(room.Name,room);
            }
        }
        //添加展厅数量
		protocol.AddInt (count);
        //遍历字典添加每个展厅信息
        foreach (Room room in templist.Values)
        {
            protocol.AddString(room.Name);//展厅名称
            protocol.AddInt(room.list.Count);//目前展厅中的人数
            protocol.AddString(room.Author);//展厅作者
        }
        return protocol;//返回协议
	}

    //获取展厅信息
    public ProtocolBytes GetRoomInfo(string name)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        //展厅的简介，这里需要减1，由于传入的值是从1开始的，而list从0开始
        protocol.AddString(list[name].Ins);
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