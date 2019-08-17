using System;
using System.Collections.Generic;

public partial class HandlePlayerMsg
{
	//开始浏览
	//public void MsgStartFight(Player player, ProtocolBase protoBase)
	//{
	//	ProtocolBytes protocol = new ProtocolBytes ();
	//	protocol.AddString ("StartFight");
	//	//条件判断
	//	if (player.tempData.status != PlayerTempData.Status.Room) 
	//	{
	//		Console.WriteLine ("MsgStartFight status err " + player.id);
	//		protocol.AddInt (-1);
	//		player.Send (protocol);
	//		return;
	//	}
		
	//	//if (!player.tempData.isOwner) 
	//	//{
	//	//	Console.WriteLine ("MsgStartFight owner err " + player.id);
	//	//	protocol.AddInt (-1);
	//	//	player.Send (protocol);
	//	//	return;
	//	//}
		
	//	Room room = player.tempData.room;
	//	//if(!room.CanStart())
	//	//{
	//	//	Console.WriteLine ("MsgStartFight CanStart err " + player.id);
	//	//	protocol.AddInt (-1);
	//	//	player.Send (protocol);
	//	//	return;
	//	//}
		
	//	//开始战斗
	//	protocol.AddInt (0);
	//	player.Send (protocol);
	//	//room.StartFight ();
	//}
    
}