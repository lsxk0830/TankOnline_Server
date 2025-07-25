/// <summary>
/// 开火
/// </summary>
public class MsgFire : MsgBase
{
    public MsgFire()
    {
        protoName = "MsgFire";
    }

    //炮弹所在位置
    public float x { get; set; } = 0;

    public float y { get; set; } = 0;
    public float z { get; set; } = 0;


    /// <summary>
    /// 哪个坦克开火的
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// 是否爆炸
    /// </summary>
    public bool IsExplosion { get; set; } = false;

    /// <summary>
    /// 子弹ID,用于区别哪个子弹发生了爆炸
    /// </summary>
    public Guid bulletID { get; set; }
}