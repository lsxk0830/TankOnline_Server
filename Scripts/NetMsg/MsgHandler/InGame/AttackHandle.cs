using System;

public partial class MsgHandler
{
    private const int damagePerHit = 35; // 每次击中造成的伤害
    public const int scale = 10000; // 客户端同比例缩放
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
            Vector3D start = new Vector3D(msg.x / scale, msg.y / scale, msg.z / scale);
            Vector3D direction = new Vector3D(msg.fx / scale, msg.fy / scale, msg.fz / scale);
            Vector3D end = new Vector3D(msg.tx / scale, msg.ty / scale, msg.tz / scale);
            // 计算起点到终点的向量
            Vector3D toEnd = end - start;
            // 归一化方向向量
            Vector3D dirNormalized = direction.Normalize();
            // 计算叉积：若终点在直线上，叉积应为零向量
            Vector3D cross = Vector3D.Cross(toEnd, dirNormalized);
            double crossMagnitudeSq = cross.X * cross.X + cross.Y * cross.Y + cross.Z * cross.Z;
            // 判断平行性（考虑浮点误差）
            Console.WriteLine($"值:{crossMagnitudeSq},1e-3:{1e-3}");
            if (crossMagnitudeSq < 1e-3 * 1e-3)
            {
                msg.fx = 0; msg.fy = 0; msg.fz = 0;
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
                        return;
                    }
                }
            }
            else
            {
                msg.isHit = false;
            }
        }
        room.Broadcast(msg);// 广播
    }
}