using System.Collections.Generic;

public class PlayerTempData
{
	public PlayerTempData()
	{
        status = Status.None;
    }
    //״̬
    public enum Status
    {
        None,
        Room,
    }
    public Status status;
    public Room room;//��ǰ�û������չ��
    //չ�����
    public long lastUpdateTime;
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
}