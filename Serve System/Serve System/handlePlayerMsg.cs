using System;

public partial class HandlePlayerMsg
{
	//获取图片、视频、模型的字节流
	//协议参数：
	//返回协议：byte数组
	public void MsgGetData(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocolRet = new ProtocolBytes ();
		protocolRet.AddString ("GetData");
        protocolRet.AddInt(player.tempData.room.picture.Count);
        foreach(Resoure resoure in player.tempData.room.picture)
        {
            protocolRet.AddByte(resoure.resourebytes);
        }
        protocolRet.AddInt(player.tempData.room.video.Count);
        foreach (Resoure resoure in player.tempData.room.video)
        {
            protocolRet.AddByte(resoure.resourebytes);
        }
        protocolRet.AddInt(player.tempData.room.model.Count);
        foreach (Resoure resoure in player.tempData.room.model)
        {
            protocolRet.AddByte(resoure.resourebytes);
        }
        player.Send (protocolRet);
		Console.WriteLine ("MsgGetData " + player.id);
	}

	//增加
	//协议参数：
	public void MsgAddData(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
        string DataSort= protocol.GetString(start, ref start);
        Resoure resoure = new Resoure();
        resoure.resourebytes = protocol.GetByte(start, ref start);
        switch (DataSort)
        {
            case "picture":
                player.tempData.room.picture.Add(resoure);
                player.data.picturecount++;
                break;
            case "video":
                player.tempData.room.video.Add(resoure);
                player.data.videocount++;
                break;
            case "model":
                player.tempData.room.model.Add(resoure);
                player.data.modelcount++;
                break;
        }
        //处理
        Console.WriteLine ("MsgAddData " + player.id);
	}

    //获取玩家列表
    public void MsgGetList(Player player, ProtocolBase protoBase)
    {
        Scene.instance.SendPlayerList(player);
    }

    //更新信息
    public void MsgUpdateInfo(Player player, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        float x = protocol.GetFloat(start, ref start);
        float y = protocol.GetFloat(start, ref start);
        float z = protocol.GetFloat(start, ref start);
        Scene.instance.UpdateInfo(player.id, x, y, z);
        //广播
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("UpdateInfo");
        protocolRet.AddString(player.id);
        protocolRet.AddFloat(x);
        protocolRet.AddFloat(y);
        protocolRet.AddFloat(z);
        ServNet.instance.Broadcast(protocolRet);
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
        Console.WriteLine("MsgGetScore " + player.id);
    }
}