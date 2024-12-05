namespace Lab2MPA.Hubs
{
    public class PresenceTracker
    {
        private static readonly HashSet<string> OnlineUsers = new HashSet<string>();

        public Task<bool> ConnectionOpened(string username)
        {
            lock (OnlineUsers)
            {
                bool isUserJoined = OnlineUsers.Add(username);
                return Task.FromResult(isUserJoined);
            }
        }

        public Task ConnectionClosed(string username)
        {
            lock (OnlineUsers)
            {
                OnlineUsers.Remove(username);
            }

            return Task.CompletedTask;
        }

        public Task<List<string>> GetOnlineUsers()
        {
            lock (OnlineUsers)
            {
                return Task.FromResult(OnlineUsers.ToList());
            }
        }
    }

}
