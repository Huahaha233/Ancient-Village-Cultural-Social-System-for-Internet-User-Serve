using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serve_System;

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
    public string Adress;//房间的地址
    public string Author;//房间的创建者

	public Dictionary<string,Player> list = new Dictionary<string,Player>();
    public Dictionary<string, Resoure> resouredata = new Dictionary<string, Resoure>();
    //添加玩家
    public bool AddPlayer(Player player)
	{
		lock (list) 
		{
			if (list.Count >= maxPlayers||list.ContainsKey(player.id))
				return false;
			PlayerTempData tempdata = player.tempData;
            tempdata.room = this; 
			player.tempData.status = PlayerTempData.Status.Room;
			string id = player.id;
			list.Add(id, player);
            Console.WriteLine("AddPlayer"+id);
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
    //删除展厅中的资源
    public bool DeleteResoure(Player player,string ResoureName,string sort)
    {
        //根据资源名称判断展厅中是否存在该资源
        if (resouredata.ContainsKey(ResoureName))
        {
            lock (resouredata)
            {
                //根据资源名称检索到资源存储地址，再根据地址删除云服务器磁盘中的资源
                File.Delete(@"C:"+resouredata[ResoureName].resoureadress);
                resouredata.Remove(ResoureName);//移除资源字典中的资源信息
                RoomMgr.instance.ReFlashPlayData(player,sort,-1);//刷新用户信息
                return true;
            }
        }
        return false;
    }
    
    //广播
    public void Broadcast(ProtocolBase protocol)
	{
		foreach(Player player in list.Values)
		{
            try
            {
                player.Send(protocol);
            }
            catch { }
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

    //增添房间的留言
    public void AddChatMessgae(string id,string message)
    {
        IHandleMysql handleMysql = new HandleMysql();
        string nowtime = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "-" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        handleMysql.insertMySQL("insert into roomchat(ChatId,RoomName,PlayerId,ChatMessage,ChatTime) values(NULL,'"+Name+"', '"+id+"', '"+message+"', '"+nowtime+"')"); //插入数据到表);
    }
    //获取房间的留言
    public Dictionary<List<string>, List<string>> GetChatMessgae()
    {
        IHandleMysql handleMysql = new HandleMysql();
        return handleMysql.selectMySQL("select * from roomchat where RoomName="+Name); //查询数据表);
    }
}