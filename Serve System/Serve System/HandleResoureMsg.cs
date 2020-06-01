using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public partial class HandlePlayerMsg
{
    //获取图片、视频、模型的信息
    //协议参数：
    //返回协议：byte数组
    public void MsgGetResoure(Player player, ProtocolBase protoBase)
    {
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("GetResoure");
        Room room = player.tempData.room;
        protocolRet.AddInt(room.resouredata.Count);
        foreach (string resoure in room.resouredata.Keys)
        {
            protocolRet.AddString(room.resouredata[resoure].resourename);
            protocolRet.AddString(room.resouredata[resoure].resoureins);
            protocolRet.AddString(room.resouredata[resoure].resouresort);
            protocolRet.AddString(room.resouredata[resoure].resoureadress);
        }
        player.Send(protocolRet);
        Console.WriteLine("MsgGetResoure " + player.id);
    }

    //获取用户选择展厅的资源信息列表
    public void MsgGetResoureList(Player player, ProtocolBase protoBase)
    {

        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        string RoomName = protocol.GetString(start, ref start);//展厅名称
        ProtocolBytes protoRet = new ProtocolBytes();
        protoRet.AddString("GetResoureList");
        protoRet.AddString(RoomName);
        //判断是否存在该展厅
        if (!RoomMgr.instance.list.ContainsKey(RoomName))
        {
            protoRet.AddInt(-1);//不存在，返回失败信息
            player.Send(protoRet);
            return;
        }
        //根据展厅名称获取展厅实体类
        Room room = RoomMgr.instance.list[RoomName];
        protoRet.AddInt(room.resouredata.Count);//添加展厅中资源数量
        //遍历展厅中的资源
        foreach (string str in room.resouredata.Keys)
        {
            protoRet.AddString(str);//添加资源名称
            protoRet.AddString(room.resouredata[str].resouresort);//添加资源类型
        }
        player.Send(protoRet);//返回资源列表给用户
        Console.WriteLine("MsgGetResoureList Ok " + player.id);//打印
    }
    

    //增加资源
    //协议参数：
    public void MsgAddResoure(Player player, ProtocolBase protoBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string RoomName = protocol.GetString(start, ref start);
        string ResoureName = protocol.GetString(start, ref start);
        string ResoureIns = protocol.GetString(start, ref start);
        string ResoureSort = protocol.GetString(start, ref start);
        string ResoureAdress = protocol.GetString(start, ref start);

        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("AddResoure");
        //判断是否存在该房间
        if (!RoomMgr.instance.list.ContainsKey(RoomName))
        {
            protocolRet.AddInt(-1);
            player.Send(protocolRet);
            return;
        }
        //判断资源名称是否存重复
        if (RoomMgr.instance.list[RoomName].resouredata.ContainsKey(ResoureName))
        {
            protocolRet.AddInt(-1);
            player.Send(protocolRet);
            return;
        }
        Room room = RoomMgr.instance.list[RoomName];
        Resoure resoure = new Resoure();
        resoure.resourename = ResoureName;
        resoure.resoureins = ResoureIns;
        resoure.resouresort = ResoureSort;
        resoure.resoureadress = ResoureAdress;
        room.resouredata.Add(ResoureName, resoure);
        player.tempData.tempadress = ResoureAdress;
        RoomMgr.instance.ReFlashPlayData(player, ResoureSort, 1);
        //处理
        protocolRet.AddInt(0);
        protocolRet.AddString(ResoureAdress);
        player.Send(protocolRet);
        Console.WriteLine("MsgAddResoure " + player.id);
        //判断资源是否上传完成        
        //Thread t = new Thread(new ParameterizedThreadStart(Judge));
        //t.Start(player);

        //ProtocolBytes protocolRet1 = new ProtocolBytes();
        //protocolRet1.AddString("UpLoadCompleted");
        ////Player player = (Player)obj;
        //while (true)
        //{
        //    //Thread.Sleep(200);//休眠
        //    if (File.Exists(@"C:" + player.tempData.tempadress)) break;
        //}
        //player.Send(protocolRet1);
    }
    private void Judge(object obj)
    {
        
    }
    //删除资源
    public void MsgDeleteResoure(Player player, ProtocolBase protoBase)
    {
        int start = 0;
        ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
        string roomname = protocol.GetString(start, ref start);//房间名称
        string resourename = protocol.GetString(start, ref start);//资源名称
        string sort = protocol.GetString(start, ref start);//类型名称
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("DeleteResoure");
        proto.AddString(roomname);
        proto.AddString(resourename);
        proto.AddString(RoomMgr.instance.list[roomname].resouredata[resourename].resoureadress);//存储地址
        if (RoomMgr.instance.list[roomname].DeleteResoure(player, resourename, sort))
        {
            proto.AddInt(0);
        }
        else proto.AddInt(-1);
        player.Send(proto);
    }

}