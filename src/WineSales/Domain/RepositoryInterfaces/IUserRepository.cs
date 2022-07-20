﻿using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IUserRepository : ICrudRepository<User>
    {
        User GetByLogin(string login);
        List<User> GetByRole(string role);
        void Register(User user);
    }
}
