using System;
using System.Collections.Generic;
using System.Linq; 

//房间
public class Room
{
	//状态
	public enum Status
	{
	}
	//玩家
	public int maxPlayers = 10;
    public string Name;//房间的名称
    public string Ins;//房间的简介
    public string Author;//房间的创建者

	public Dictionary<string,Player> list = new Dictionary<string,Player>();
    public Dictionary<string, Resoure> picture = new Dictionary<string, Resoure>();
    public Dictionary<string, Resoure> video = new Dictionary<string, Resoure>();
    public Dictionary<string, Resoure> model = new Dictionary<string, Resoure>();
    //添加玩家
    public bool AddPlayer(Player player)
	{
		lock (list) 
		{
			if (list.Count >= maxPlayers)
				return false;
			PlayerTempData tempdata = player.tempData;
            tempdata.room = this; 
			player.tempData.status = PlayerTempData.Status.Room;

			string id = player.id;
			list.Add(id, player);
		}
        return true;
	}
	//删除玩家
	public bool DelPlayer(string id)
	{
		lock (list) 
		{
			if (!list.ContainsKey(id))
				return false;
			list.Remove(id);
		}
        return true;
	}
    //删除房间中的资源
    public bool DeleteResoure(Player player,string ResoureName,string sort)
    {
        switch (sort)
        {
            case "图片":
                if (picture.ContainsKey(ResoureName))
                {
                    lock (picture)
                    {
                        picture.Remove(ResoureName);
                        player.data.picturecount--;
                        return true;
                    }
                }
                return false;
            case "视频":
                if (video.ContainsKey(ResoureName))
                {
                    lock (video)
                    {
                        video.Remove(ResoureName);
                        player.data.picturecount--;
                        return true;
                    }
                }
                return false;
            case "3D模型":
                if (model.ContainsKey(ResoureName))
                {
                    lock (model)
                    {
                        model.Remove(ResoureName);
                        player.data.picturecount--;
                        return true;
                    }
                }
                return false;
        }
        return false;
    }
    
    //广播
    public void Broadcast(ProtocolBase protocol)
	{
		foreach(Player player in list.Values)
		{
			player.Send(protocol);
		}
	}
    //判断资源类型
    public Dictionary<string, Resoure> JudegeSort(string sort)
    {
        switch (sort)
        {
            case "picture":
                return picture;
            case "video":
                return video;
            case "model":
                return model;
        }
        return null;
    }
    //开始浏览
    public ProtocolBytes StartVisit()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartVisit");
        lock (list)
        {
            protocol.AddInt(list.Count);
            foreach (Player p in list.Values)
            {
                protocol.AddString(p.id);
                protocol.AddFloat(p.tempData.posX);
                protocol.AddFloat(p.tempData.posY);
                protocol.AddFloat(p.tempData.posZ);
                protocol.AddFloat(p.tempData.rotX);
                protocol.AddFloat(p.tempData.rotY);
                protocol.AddFloat(p.tempData.rotZ);
            }
            return protocol;
        }
    }
}