/// <summary>
/// 同步坦克信息
/// </summary>
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank()
    {
        protoName = "MsgStartBattle";
    }

    //位置
    public int x { get; set; } = 0;

    public int y { get; set; } = 0;
    public int z { get; set; } = 0;

    //旋转
    public int ex { get; set; } = 0;

    public int ey { get; set; } = 0;
    public int ez { get; set; } = 0;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public int turretY { get; set; } = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public long id { get; set; }
}