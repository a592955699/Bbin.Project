using System;
using System.Collections.Generic;
using System.Text;

namespace Bbin.Core.Cons
{
    public class RoomCons
    {
        public static Dictionary<string, string> Rooms = new Dictionary<string, string>()
        {
            { "3001-1", "百家乐A" },
            { "3001-2", "百家乐B" },
            { "3001-3", "百家乐C" },
            { "3001-6", "百家乐D" },
            { "3001-7", "百家乐E" },
            { "3001-54", "百家乐F" },
            //{ "3001-8", "百家乐K" },
            //{ "3001-37", "极速百家乐J" },
            //{ "3001-55", "3001-55" },
            //{ "3001-56", "百家乐H" },
            //{ "3001-57", "3001-57" },
            //{ "3001-58", "百家乐EU2" },
            //{ "3001-59", "3001-59" },
            //{ "3001-65", "3001-65" },
            //{ "3001-66", "3001-66" },
            //{ "3001-67", "3001-67" },
            //{ "3001-68", "3001-68" },
            //{ "3001-69", "3001-69" },
            //{ "3001-70", "3001-70" }
        };

        public static string GetRoomName(string roomeId)
        {
            if (Rooms.ContainsKey(roomeId))
            {
                return Rooms[roomeId];
            }
            else
            {
                return roomeId;
            }
        }
    }
}
