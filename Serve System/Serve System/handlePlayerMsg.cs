using System;

public partial class HandlePlayerMsg
{
	//获取图片、视频、模型的字节流
	//协议参数：
	//返回协议：byte数组
	public void MsgGetResoure(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocolRet = new ProtocolBytes ();
		protocolRet.AddString ("GetResoure");
        Room room = player.tempData.room;
        protocolRet.AddInt(room.resouredata.Count);
        foreach(string resoure in room.resouredata.Keys)
        {
            protocolRet.AddString(room.resouredata[resoure].resourename);
            protocolRet.AddString(room.resouredata[resoure].resoureins);
            protocolRet.AddString(room.resouredata[resoure].resouresort);
            protocolRet.AddString(room.resouredata[resoure].resoureadress);
        }
        player.Send (protocolRet);
		Console.WriteLine ("MsgGetResoure " + player.id);
	}
	
    
    //获取玩家信息
    public void MsgGetAchieve(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("GetAchieve");
        protocolRet.AddInt(player.data.picturecount);
        protocolRet.AddInt(player.data.videocount);
        protocolRet.AddInt(player.data.modelcount);
        player.Send(protocolRet);
        Console.WriteLine("MsgGetAchieve " + player.id);
    }
}