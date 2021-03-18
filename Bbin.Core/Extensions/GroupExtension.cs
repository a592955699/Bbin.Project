namespace Bbin.Core.Extensions
{
    public static class GroupExtension
    {
        public static string GetGroupName(string roomId)
        {
            return $"room_{roomId}";
        }
    }
}
