using System;
using System.Collections.Generic;

public partial class HandlePlayerMsg
{
    //开始浏览
    public void MsgStartVisit(Player player, ProtocolBase protoBase)
    {
        //ProtocolBytes protocol = new ProtocolBytes();
        //protocol.AddString("StartVisit");
        //条件判断
        //if (player.tempData.status != PlayerTempData.Status.Room)
        //{
        //    Console.WriteLine("MsgStartFight status err " + player.id);
        //    protocol.AddInt(-1);
        //    player.Send(protocol);
        //    return;
        //}
        Room room = player.tempData.room;
        //开始战斗
        //protocol.AddInt(0);
        //player.Send(protocol);
        room.StartVisit();
    }

}