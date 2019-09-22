using System.Collections.Generic;

public class PlayerTempData
{
	public PlayerTempData()
	{
        status = Status.None;
    }
    //状态
    public enum Status
    {
        None,
        Room,
    }
    public Status status;
    public List<Room> rooms;//当前用户创建的房间
    public Room room;//当前用户进入的展厅
    //展厅相关
    public long lastUpdateTime;
    public float posX;
    public float posY;
    public float posZ;
    //public long lastShootTime;
    //public float hp = 100;
}