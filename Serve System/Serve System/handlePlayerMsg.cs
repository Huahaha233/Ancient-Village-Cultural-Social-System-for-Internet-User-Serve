using System;

public partial class HandlePlayerMsg
{
    
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