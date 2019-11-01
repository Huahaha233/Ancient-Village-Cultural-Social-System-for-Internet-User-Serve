using System;
using System.Collections.Generic;

public partial class HandlePlayerMsg
{
    //开始浏览
    public void MsgStartVisit(Player player, ProtocolBase protoBase)
    {
        Room room = player.tempData.room;
        player.Send(room.StartVisit());
    }
}