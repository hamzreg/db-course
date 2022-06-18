using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface IUserInteractor
    {
        void Create(User user);
        void Update(User user);
        void Delete(User user);
        void Register(string login, string password, string role);
        void SignIn(string login, string password, string role);
        int GetNowUserID();
        User GetNowUser();
        void SetNowUser(User user);
    }

    public class UserInteractor : IUserInteractor
    {
        private readonly IUserRepository userRepository;
        private User nowUser;

        public UserInteractor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            nowUser = new User(UserConfig.Default,
                               UserConfig.Default,
                               UserConfig.Roles[3]);
        }

        public int GetNowUserID()
        {
            return nowUser.ID;
        }

        public User GetNowUser()
        {
            return nowUser;
        }

        public void SetNowUser(User user)
        {
            nowUser = user;
        }

        public void Create(User user)
        {
            if (Exist(user))
                throw new UserException("This user already exists.");

            if (!CheckPassword(user.Password))
                throw new UserException("Invalid input of password.");
        }

        public void Update(User user)
        {
            if (!Exist(user))
                throw new UserException("This user doesn't exist.");

            if (user.Password != null && !CheckPassword(user.Password))
                throw new UserException("Invalid input of password.");

            userRepository.Update(user);
        }

        public void Delete(User user)
        {
            if (!Exist(user))
                throw new UserException("This user doesn't exist.");

            userRepository.Delete(user);
        }

        public void Register(string login, string password, string role)
        {
            if (!CheckPassword(password))
                throw new UserException("Invalid input of password.");

            var user = new User(login, password, role);

            if (Exist(user))
                throw new UserException("This user already exists.");

            userRepository.Register(user);
        }

        public void SignIn(string login, string password, string role)
        {
            if (!CheckPassword(password))
                throw new UserException("Invalid input of password.");

            var user = new User(login, password, role);

            var authorizedUser = userRepository.GetByLogin(login);

            if (authorizedUser == null)
                throw new UserException("This user doesn't exist.");

            if (password != authorizedUser.Password)
                throw new UserException("Invalid password.");

            if (!userRepository.ConnectToDataStore(authorizedUser))
                throw new UserException("Failed to connect to data storage.");

            SetNowUser(user);
        }

        private bool Exist(User user)
        {
            return userRepository.GetByLogin(user.Login) != null;
        }

        private bool CheckPassword(string password)
        {
            return UserConfig.MinPasswordLen < password.Length;
        }
    }
}
