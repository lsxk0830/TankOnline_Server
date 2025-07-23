public partial class MsgHandler
{
    private const int damagePerHit = 35; // 每次击中造成的伤害

    /// <summary>
    /// 击中协议
    /// </summary>
    public static void MsgHit(ClientState cs, MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;
        //Player? attackPlayer = room.GetPlayer(msg.id); // 攻击者
        Player? hitPlayer = room.GetPlayer(msg.targetId);// 被击中者
        //if (attackPlayer == null) return;
        if (hitPlayer == null) return;
        if ((Room.Status)room.status != Room.Status.FIGHT) return;

        // 状态
        Console.WriteLine($"击中协议");
        hitPlayer.hp -= damagePerHit;
        msg.hp = hitPlayer.hp;
        msg.damage = damagePerHit;
        room.Broadcast(msg);// 广播
        Console.WriteLine($"击中协议:{msg.hp}");

        if (hitPlayer.hp <= 0)
        {
            int winCamp = room.Judgment(hitPlayer.camp, hitPlayer.ID);
            if (winCamp != 0)
            {
                // 游戏结束
                Console.WriteLine($"发送游戏结束协议,删除房间:{user.RoomID}");
                room.Broadcast(new MsgEndBattle()
                {
                    winCamp = winCamp
                });
                // 更新数据库
                List<User> users = new List<User>(room.playerIds.Count);
                foreach (var player in room.playerIds)
                {
                    User? playerUser = UserManager.GetUser(player.Key);
                    if (playerUser == null) continue;
                    if (player.Value.camp == winCamp)
                        playerUser.Win++;
                    else
                        playerUser.Lost++;
                    users.Add(playerUser);
                }
                DbManager.BatchUpdateUsers(users); 
                RoomManager.RemoveRoom(user.RoomID);
            }
        }
    }
}