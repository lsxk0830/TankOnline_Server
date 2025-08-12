public partial class MsgHandler
{
    private const int damagePerHit = 35; // 每次击中造成的伤害

    /// <summary>
    /// 击中协议
    /// </summary>
    public static void MsgAttack(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"攻击协议");
        MsgAttack msg = (MsgAttack)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;
        Player? hitPlayer = room.GetPlayer(msg.hitID);// 被击中者
        if(hitPlayer == null) return;
        if ((Room.Status)room.status != Room.Status.FIGHT) return;
            
        /*
        if ((msg.fx == 0 && msg.fy == 0 && msg.fz == 0) || hitPlayer == null)
        {
            msg.isHit = false;
        }
        else
        {
            // 计算向量
            int dx = msg.tx - msg.x;
            int dy = msg.ty - msg.y;
            int dz = msg.tz - msg.z;

            // 计算三个方向的交叉乘积
            long crossXY = (long)dx * msg.fy - (long)dy * msg.fx; // long ：dx * fz可能超过int.MaxValue
            long crossYZ = (long)dy * msg.fz - (long)dz * msg.fy;
            long crossXZ = (long)dx * msg.fz - (long)dz * msg.fx;

            // 计算最大可能的误差范围（1%容忍度）
            long toleranceXY = Math.Max(Math.Abs((long)dx * msg.fy), Math.Abs((long)dy * msg.fx)) / 100;
            long toleranceYZ = Math.Max(Math.Abs((long)dy * msg.fz), Math.Abs((long)dz * msg.fy)) / 100;
            long toleranceXZ = Math.Max(Math.Abs((long)dx * msg.fz), Math.Abs((long)dz * msg.fx)) / 100;

            // 判断是否在容忍范围内
            bool isCollinear = Math.Abs(crossXY) <= toleranceXY &&
                           Math.Abs(crossYZ) <= toleranceYZ &&
                           Math.Abs(crossXZ) <= toleranceXZ;

            // 检查方向是否一致（避免反向命中）
            bool isSameDirection = (dx * msg.fx + dy * msg.fy + dz * msg.fz) > 0;
            //Console.WriteLine($"dx={dx}, dy={dy}, dz={dz}");
            //Console.WriteLine($"fx={msg.fx}, fy={msg.fy}, fz={msg.fz}");
            //Console.WriteLine($"dx*fy={dx * msg.fy}, dy*fx={dy * msg.fx}");
            //Console.WriteLine($"dy*fz={dy * msg.fz}, dz*fy={dz * msg.fy}");
            //Console.WriteLine($"dx*fz={dx * msg.fz}, dz*fx={dz * msg.fx}");
            //Console.WriteLine($"isCollinear:{isCollinear},isSameDirection:{isSameDirection}");
            // 检查共线性
            //const double tolerance = 1e-4;
            //bool isCollinear = Math.Abs(dx - msg.fx * ratio) < tolerance && Math.Abs(dy - msg.fy * ratio) < tolerance && Math.Abs(dz - msg.fz * ratio) < tolerance;
            //Console.WriteLine($"ratio:{ratio},{Math.Abs(dx - msg.fx * ratio)},{Math.Abs(dy - msg.fy * ratio)},{Math.Abs(dz - msg.fz * ratio)}");
            if (isSameDirection && isCollinear)
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
        */
        if(msg.isHit)
        {
            msg.fx = 0; msg.fy = 0; msg.fz = 0;
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
        room.Broadcast(msg);// 广播
    }
}