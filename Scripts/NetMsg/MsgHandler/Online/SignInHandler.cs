public partial class MsgHandler
{
    /// <summary>
    /// 签到协议处理
    /// </summary>
    public static void MsgSignIn(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine("签到协议"); ;
        MsgSignIn msg = (MsgSignIn)msgBase;
        if (cs.user == null)
        {
            Console.WriteLine("用户未登录，无法签到");
            return;
        }
        if (cs.user.LastSignIn == DateTime.Today)
        {
            Console.WriteLine("用户今天已经签到过了");
            msg.Query = false;
            msg.ContinuousSignIn = cs.user.ContinuousSignIn;
            NetManager.Send(cs, msg);
            return;
        }

        if (msg.Query) // 查询签到信息
        {
            if (cs.user.LastSignIn.AddDays(1) == DateTime.Today && cs.user.ContinuousSignIn == 7)
                cs.user.ContinuousSignIn = 0;
            else if (cs.user.LastSignIn.AddDays(1) != DateTime.Today)
            {
                cs.user.ContinuousSignIn = 0;
            }
            msg.ContinuousSignIn = cs.user.ContinuousSignIn;
            NetManager.Send(cs, msg);
            return;
        }

        if (cs.user.LastSignIn.AddDays(1) == DateTime.Today)
        {
            cs.user.ContinuousSignIn++;
            if (cs.user.ContinuousSignIn > 7)
            {
                cs.user.ContinuousSignIn = 1;
            }
        }
        else
        {
            cs.user.ContinuousSignIn = 1;
        }
        cs.user.LastSignIn = DateTime.Today;

        switch (cs.user.ContinuousSignIn)
        {
            case 1:
                cs.user.Coin += 100;
                break;
            case 2:
                cs.user.Diamond += 100;
                break;
            case 3:
                cs.user.Coin += 200;
                break;
            case 4:
                cs.user.Diamond += 200;
                break;
            case 5:
                cs.user.Coin += 300;
                break;
            case 6:
                cs.user.Diamond += 300;
                break;
            case 7:
                cs.user.Coin += 500;
                cs.user.Diamond += 500;
                break;
            default:
                Console.WriteLine("签到天数异常");
                return;
        }

        NetManager.Send(cs, msg);
        DbManager.UpdateSignIn(cs.user);
    }
}