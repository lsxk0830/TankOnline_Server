public partial class MsgHandler
{
    /// <summary>
    /// 同步位置协议
    /// </summary>
    public static void MsgSyncTank(ClientState cs, MsgBase msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;
        Player? player = room.GetPlayer(user.ID);
        if (player == null) return;
        if ((Room.Status)room.status != Room.Status.FIGHT) return;

        // 更新信息
        player.x = msg.x;
        player.y = msg.y;
        player.z = msg.z;
        player.ex = msg.ex;
        player.ey = msg.ey;
        player.ez = msg.ez;
        player.turretY = msg.turretY;
        //Console.WriteLine($"同步位置协议:{JsonConvert.SerializeObject(player)}");
        // 广播
        msg.id = player.ID;
        room.BroadcastExceptCS(user.ID, msg);
    }
}