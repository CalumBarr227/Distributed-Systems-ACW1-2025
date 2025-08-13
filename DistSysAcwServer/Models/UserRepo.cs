//using Microsoft.Identity.Client;

//namespace DistSysAcwServer.Models
//{
//    public class UserRepo
//    {
//        private readonly UserContext _context;

//        public UserRepo(UserContext context)
//        {
//            _context = context;
//        }

//        public bool UserExists(string username)
//        {
//            return _context.Users.Any(u => u.UserName == username);
//        }

//        public int GetUserCount()
//        {
//            return _context.Users.Count();
//        }


//        public User CreateUser(string username, string role)
//        {
//            var user = new User
//            {
//                ApiKey = Guid.NewGuid().ToString(),
//                UserName = username,
//                Role = role
//            };

//            _context.Users.Add(user);
//            _context.SaveChanges();

//            return user;
//        }
//    }
//}



using Microsoft.Identity.Client;

namespace DistSysAcwServer.Models
{
    public class UserRepo
    {
        private readonly UserContext _context;

        public UserRepo(UserContext context)
        {
            _context = context;
        }

        public User CreateUser(string username, string role)
        {
            var user = new User
            {
                ApiKey = Guid.NewGuid().ToString(),
                UserName = username,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public bool UserExistsByApiKey(string apiKey)
        {
            return _context.Users.Any(u => u.ApiKey == apiKey);
        }

        public bool UserExistsByApiKeyAndUsername(string apiKey, string username)
        {
            return _context.Users.Any(u => u.ApiKey == apiKey && u.UserName == username);
        }

        public User? GetUserByApiKey(string apiKey)
        {
            return _context.Users.FirstOrDefault(u => u.ApiKey == apiKey);
        }

        public bool DeleteUserByApiKey(string apiKey)
        {
            var user = _context.Users.FirstOrDefault(u => u.ApiKey == apiKey);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public int GetUserCount()
        {
            return _context.Users.Count();
        }

        public bool UserExists(string username)
        {
            return _context.Users.Any(u => u.UserName == username);
        }
    }
}
