using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public List<Room> rooms;//当前用户创建的房间
    public int picturecount = 0;
    public int videocount = 0;
    public int modelcount = 0;
    public PlayerData()
	{
        
	}
}