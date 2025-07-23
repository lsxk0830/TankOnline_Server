﻿public partial class MsgHandler
{
    /// <summary>
    /// 创建房间
    /// </summary>
    public static void MsgCreateRoom(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"接收:MsgCreateRoom协议");

        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        User? user = cs.user;
        if (user == null) return;

        Room room = RoomManager.CreateRoom(); // 创建房间
        Player player = new Player(cs)
        {
            ID = user.ID,
            Name = user.Name,
            AvatarPath = user.AvatarPath,
            Win = user.Win,
            Lost = user.Lost,
        };
        bool result = room.AddPlayer(player);
        msg.result = result ? 0 : -1;
        msg.room = room;
        cs.user.RoomID = room.RoomID;
        NetManager.Send(cs, msg); //返回创建房间的结果
        UserManager.SendExcept(cs, msg); // 全员通知
    }
}