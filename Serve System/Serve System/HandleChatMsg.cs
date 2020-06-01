using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public partial class HandlePlayerMsg
{
    //增加展厅留言
    public void MsgAddRoomChat(Player player, ProtocolBase protoBase)
    {
        //初始化数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string ChatMessage = protocol.GetString(start, ref start);//解码留言
        //获取展厅
        Room room = player.tempData.room;
        room.AddChatMessgae(player.id, ChatMessage);//将留言记录增添到数据库
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("AddRoomChat");//添加协议名
        protocolRet.AddString(player.id);//添加留言者
        protocolRet.AddString(ChatMessage);//添加留言
        room.Broadcast(protocolRet);//广播
    }

    //获取展厅中所有历史留言
    public void MsgGetRoomChat(Player player, ProtocolBase protoBase)
    {
        //获取展厅类
        Room room = player.tempData.room;
        //从数据库获取该展厅所有留言
        Dictionary<List<string>, List<string>> chat = room.GetChatMessgae();
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("GetRoomChat");
        //循环添加使用留言到协议
        foreach (List<string> key in chat.Keys)
        {
            protocolRet.AddInt(key.Count);//留言数目
            foreach (List<string> value in chat.Values)
            {
                for(int i=0;i< key.Count;i++)
                {
                    protocolRet.AddString(key[i]);//留言者
                    protocolRet.AddString(value[i]);//留言
                }
            }
        }
        try
        {
            player.Send(protocolRet);//返回历史留言给用户
        }
        catch
        { }
    }
}