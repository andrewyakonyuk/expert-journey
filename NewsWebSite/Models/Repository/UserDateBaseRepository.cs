using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace NewsWebSite.Models.Repository
{
    public class UserDateBaseRepository:IUserRepository
    {
        readonly ISessionFactory sessionFactory;
        public UserDateBaseRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }
        public int Save(UserModels user)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(user);
                    transaction.Commit();
                    return user.Id;
                }
            }
        }
        public UserModels GetUserById(int Id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<UserModels>(Id);
            }
        }
        public IEnumerable<UserModels> GetAllUsers()
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<UserModels>().List<UserModels>();
            }
        }
    }
}