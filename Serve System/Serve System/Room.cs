using System;
using System.Collections.Generic;
using System.Linq; 

//房间
public class Room
{
	//状态
	public enum Status
	{
		//Prepare = 1,
		//Visit = 2 ,
	}
	//public Status status = Status.Prepare;
	//玩家
	public int maxPlayers = 10;
    public string Name;//房间的名称
    public string Ins;//房间的简介
    public string Author;//房间的创建者

	public Dictionary<string,Player> list = new Dictionary<string,Player>();
    public List<Resoure> picture = new List<Resoure>();
    public List<Resoure> video = new List<Resoure>();
    public List<Resoure> model = new List<Resoure>();
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
			list[id].tempData.status = PlayerTempData.Status.None;
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
                return DeleteResoure(picture,ResoureName);
            case "视频":
                return DeleteResoure(video, ResoureName);
            case "3D模型":
                return DeleteResoure(model, ResoureName);
        }
        return false;
    }
    //删除房间中的资源
    private bool DeleteResoure(List<Resoure> resoures,string ResoureName)
    {
        Resoure resoure = new Resoure();
        foreach (Resoure r in resoures)
        {
            if (r.resourename == ResoureName)
            {
                resoure = r;
                break;
            }
        }
        if (resoure != null)
        {
            lock (resoures)
            {
                resoures.Remove(resoure);
                return true;
            }
        }
        else return false;
    }
    //广播
    public void Broadcast(ProtocolBase protocol)
	{
		foreach(Player player in list.Values)
		{
			player.Send(protocol);
		}
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