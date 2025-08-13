/// <summary>
/// 签到
/// </summary>
public class MsgSignIn : MsgBase
{
    public MsgSignIn()
    {
        protoName = "MsgSignIn";
    }

    /// <summary>
    /// 查询签到信息
    /// </summary>
    public bool Query { get; set; }

    /// <summary>
    /// 连续签到天数
    /// </summary>
    public int ContinuousSignIn { get; set; }
}