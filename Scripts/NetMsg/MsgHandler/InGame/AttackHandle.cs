using System;

public partial class MsgHandler
{
    private const int damagePerHit = 35; // 每次击中造成的伤害

    /// <summary>
    /// 击中协议
    /// </summary>
    public static void MsgAttack(ClientState cs, MsgBase msgBase)
    {
        MsgAttack msg = (MsgAttack)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;
        Player? hitPlayer = room.GetPlayer(msg.hitID);// 被击中者
        if ((Room.Status)room.status != Room.Status.FIGHT) return;

        Console.WriteLine($"攻击协议");
        if ((msg.fx == 0 && msg.fy == 0 && msg.fz == 0 && msg.tx == 0 && msg.ty == 0 && msg.tz == 0) || hitPlayer == null)
        {
            msg.isHit = false;
        }
        else
        {
            // 计算两个向量：起点到方向点，起点到击中点
            float vx = msg.fx - msg.x, vy = msg.fy - msg.y, vz = msg.fz - msg.z;
            float wx = msg.tx - msg.x, wy = msg.ty - msg.y, wz = msg.tz - msg.z;

            // 计算叉积
            float crossX = vy * wz - vz * wy;
            float crossY = vz * wx - vx * wz;
            float crossZ = vx * wy - vy * wx;

            // 计算叉积的模长平方（避免开平方运算）
            float crossMagnitudeSquared = crossX * crossX + crossY * crossY + crossZ * crossZ;

            if (crossMagnitudeSquared < 1e-4f * 1e-4f)
            {
                msg.isHit = true;
                hitPlayer.hp -= damagePerHit;
                msg.hp = hitPlayer.hp;
                msg.damage = damagePerHit;
                
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
            else
            {
                msg.isHit = false;
            }
            room.Broadcast(msg);// 广播
        }
    }
}