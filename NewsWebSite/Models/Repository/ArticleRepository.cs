using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWebSite.Models.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        readonly ISessionFactory sessionFactory;

        public ArticleRepository(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public void Delete(int articleId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.Delete(session.Load<Article>(articleId));
                    t.Commit();
                }
            }
        }


        public int Save(Article a)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    var timeNow = DateTime.Now;
                    if (a.CreateDate == DateTime.MinValue)
                        a.CreateDate = timeNow;
                    a.LastUpdateDate = timeNow;
                    session.SaveOrUpdate(a);
                    t.Commit();
                    return a.Id;
                }
            }
        }

        public Article GetItem(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.Get<Article>(id);
            }
        }

        public bool IsExist(int id)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var count = session.CreateCriteria<Article>()
                    .SetProjection(Projections.RowCount())
                    .Add(Restrictions.IdEq(id))
                    .UniqueResult<int>();
                return count == 1;
            }
        }

        public bool IsAuthor(int articleId, int userId)
        {
            using (var session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria<Article>()
                    .SetProjection(Projections.RowCount())
                    .Add(Restrictions.IdEq(articleId))
                    .Add(Restrictions.Eq("UserId", userId))
                    .UniqueResult<int>() == 1;
            }
        }

        public PagedList<DemoArticle> GetDemoList(NewsCriteria cr)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var filter = session.CreateCriteria<Article>();
                if (cr.LastId > 0) filter.Add(Restrictions.Lt("Id", cr.LastId));
                else filter.SetFirstResult(cr.StartFrom);

                var results = new PagedList<DemoArticle>();
                var countCreteria = (ICriteria)filter.Clone();
                results.AddRange(filter
                     .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Id(), "Id")
                    .Add(Projections.Property("Title"), "Title")
                    .Add(Projections.Property("Image"), "Image")
                    .Add(Projections.Property("CreateDate"), "CreateDate")
                    .Add(Projections.Property("LastUpdateDate"), "LastUpdateDate"))
                    .AddOrder(Order.Desc("Id"))
                    .SetMaxResults(cr.Count)
                    .SetResultTransformer(Transformers.AliasToBean<DemoArticle>())
                    .List<DemoArticle>());

                results.LinesCount = countCreteria.SetProjection(Projections.RowCount()).UniqueResult<int>();

                results.PageCount = (int)Math.Ceiling(results.LinesCount / (double)cr.Count);
                return results;
            }
        }


        public int GetCountOfLines()
        {
            using (var session = sessionFactory.OpenSession())
            {
                var count = session.QueryOver<Article>().Select(Projections.RowCount()).FutureValue<int>().Value;
                return count;
            }
        }
    }



    public class NewsCriteria
    {
        public int StartFrom { get; set; }
        public int Count { get; set; }
        public int LastId { get; set; }
        public NewsCriteria()
        {
            Count = 10;
        }
    }
}