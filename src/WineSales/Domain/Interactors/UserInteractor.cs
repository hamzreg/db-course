using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface IUserInteractor
    {
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        void Register(LoginDetails info, string role);
        void SignIn(LoginDetails info);
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

        public void CreateUser(User user)
        {
            if (Exist(user.Login))
                throw new UserException("This user already exists.");

            if (!CheckPassword(user.Password))
                throw new UserException("Invalid input of password.");

            userRepository.Create(user);
        }

        public void UpdateUser(User user)
        {
            if (NotExist(user.ID))
                throw new UserException("This user doesn't exist.");

            if (user.Login != null && Exist(user.Login))
                throw new UserException("This login is already in use.");

            if (user.Password != null && !CheckPassword(user.Password))
                throw new UserException("Invalid input of password.");

            userRepository.Update(user);
        }

        public void DeleteUser(User user)
        {
            if (NotExist(user.ID))
                throw new UserException("This user doesn't exist.");

            userRepository.Delete(user);
        }

        public void Register(LoginDetails info, string role)
        {
            if (Exist(info.Login))
                throw new UserException("This user already exists.");

            if (!CheckPassword(info.Password))
                throw new UserException("Invalid input of password.");

            var newUser = new User(info.Login, info.Password, role);
            userRepository.Register(newUser);
        }

        public void SignIn(LoginDetails info)
        {
            if (!CheckPassword(info.Password))
                throw new UserException("Invalid input of password.");

            var authorizedUser = userRepository.GetByLogin(info.Login);

            if (authorizedUser == null)
                throw new UserException("This user doesn't exist.");

            if (info.Password != authorizedUser.Password)
                throw new UserException("Invalid password.");

            SetNowUser(authorizedUser);
        }

        private bool Exist(string login)
        {
            return userRepository.GetByLogin(login) != null;
        }

        private bool NotExist(int id)
        {
            return userRepository.GetByID(id) == null;
        }

        private bool CheckPassword(string password)
        {
            return UserConfig.MinPasswordLen <= password.Length;
        }
    }
}
