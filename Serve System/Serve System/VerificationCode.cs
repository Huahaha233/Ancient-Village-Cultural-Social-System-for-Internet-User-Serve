using System;
using System.Security.Cryptography;
using Random = System.Random;

public class VerificationCode{

    private string text = "";
    /// <summary>
    /// 验证码
    /// </summary>
    public string Text
    {
        get { return this.text; }
    }

    public VerificationCode(int length)
    {
        Number(length, false);
    }


    /// <summary>
    /// 生成随机数字
    /// </summary>
    /// <param name="Length">生成长度</param>
    /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
    private string Number(int Length, bool Sleep)
    {
        if (Sleep) System.Threading.Thread.Sleep(3);
        string result = "";
        System.Random random = new System.Random();
        for (int i = 0; i < Length; i++)
        {
            result += random.Next(10).ToString();
        }
        this.text = result;
        //Debug.Log("result: " + result);
        return result;
    }

    /*产生验证码*/
    public string CreateCode(int codeLength)
    {
        string so = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
        string[] strArr = so.Split(',');
        string code = "";
        Random rand = new Random();
        for (int i = 0; i < codeLength; i++)
        {
            code += strArr[rand.Next(0, strArr.Length)];
        }
        return code;
    }
}
