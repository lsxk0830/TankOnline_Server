/// <summary>
/// 击中
/// </summary>
public class MsgAttack : MsgBase
{
    public MsgAttack()
    {
        protoName = "MsgAttack";
    }

    //炮弹初始位置
    public int x { get; set; } = 0;
    public int y { get; set; } = 0;
    public int z { get; set; } = 0;

    //炮弹方向
    public int fx { get; set; } = 0;
    public int fy { get; set; } = 0;
    public int fz { get; set; } = 0;

    //击中点位置
    public int tx { get; set; } = 0;
    public int ty { get; set; } = 0;
    public int tz { get; set; } = 0;

    /// <summary>
    /// 哪个坦克开火的
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// 哪个坦克受伤
    /// </summary>
    public long hitID { get; set; }

    /// <summary>
    /// 网络判断是否受伤
    /// </summary>
    public bool isHit { get; set; }

    /// <summary>
    /// 被击中坦克血量
    /// </summary>
    public int hp { get; set; }

    /// <summary>
    /// 受到的伤害
    /// </summary>
    public int damage { get; set; }
}