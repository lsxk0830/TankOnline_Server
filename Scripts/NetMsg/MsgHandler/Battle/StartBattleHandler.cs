﻿public partial class MsgHandler
{
    /// <summary>
    /// 请求开始战斗
    /// </summary>
    public static void MsgStartBattle(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"请求开始战斗");
        MsgStartBattle msg = (MsgStartBattle)msgBase;

        if (cs.user == null)
        {
            Console.WriteLine("用户请求开始战斗错误，无法开始战斗");
            msg.result = -1;
            NetManager.Send(cs, msg);
            return;
        }
        Console.WriteLine($"玩家：{cs.user.Name} 请求开始战斗");
        Room room = RoomManager.GetRoom(msg.roomID);

        if (room == null) // Room是否存在
        {
            msg.result = -1;
            NetManager.Send(cs, msg);
            return;
        }

        if (room.ownerId != cs.user.ID) // 是否是房主
        {
            msg.result = -1;
            NetManager.Send(cs, msg);
            return;
        }
        if (!room.CanStartBattle()) // 是否是房主
        {
            msg.result = -1;
            NetManager.Send(cs, msg);
            return;
        }
        room.status = (int)Room.Status.FIGHT; // 状态设置为战斗中
        room.BroadcastExceptCS(cs.user.ID, msg); // 广播开战消息
        UserManager.Send(RoomManager.GetRoomsToMsg());
    }
}