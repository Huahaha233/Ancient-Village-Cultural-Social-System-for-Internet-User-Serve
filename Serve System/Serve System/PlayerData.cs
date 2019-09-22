using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int picturecount;
    public int videocount;
    public int modelcount;
    public PlayerData()
	{
        picturecount = 0;
        videocount = 0;
        modelcount = 0;
    }
}