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
    public Room room;//当前用户进入的展厅
    public string tempadress;//当前上传资源的地址
    public string mapadress;//当前用户在地图中选择的地址
    //展厅相关
    public long lastUpdateTime;
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
}