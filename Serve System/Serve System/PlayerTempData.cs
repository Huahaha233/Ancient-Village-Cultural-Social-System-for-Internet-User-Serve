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
    public List<Room> rooms;//��ǰ�û������ķ���
    public Room room;//��ǰ�û������չ��
    //չ�����
    public long lastUpdateTime;
    public float posX;
    public float posY;
    public float posZ;
    //public long lastShootTime;
    //public float hp = 100;
}