using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebSite.Models.Repository
{
    public interface IUserRepository
    {
        int Save(UserModels user);
        UserModels GetUserById(int Id);
        IEnumerable<UserModels> GetAllUsers();
    }
}
