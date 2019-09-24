using System;

public class Player
{
	//id、连接、玩家数据
	public string id;
	public Conn conn;
	public PlayerData data;
	public PlayerTempData tempData;

	//构造函数，给id和conn赋值
	public Player(string id, Conn conn)
	{
		this.id = id;
		this.conn = conn;
		tempData = new PlayerTempData();
	}
	
	//发送
	public void Send(ProtocolBase proto)
	{
		if (conn == null)
			return;
		ServNet.instance.Send(conn, proto);
	}
	
	//是否在线
	public static bool KickOff(string id)
	{
		Conn[] conns = ServNet.instance.conns;
		for (int i = 0; i < conns.Length; i++)
		{
			if (conns[i] == null)
				continue;
			if (!conns[i].isUse)
				continue;
			if (conns[i].player == null)
				continue;
			if (conns[i].player.id == id)
			{
                return true;
			}
		}
		return false;
	}
	
	//下线
	public bool Logout()
	{
        //事件处理，稍后实现
        //ServNet.instance.handlePlayerEvent.OnLogout(this);
        //保存
        DataMgr.instance.SavePlayer(this);
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Logout");
        protocol.AddInt(0);
        conn.Send(protocol);
        //下线
        conn.player = null;
        conn.Close();
		return true;
	}
}