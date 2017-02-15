using NewsWebSite.Models.SignalrModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebSite.Models.Repository
{
    public interface ICommentsRepository
    {
        Comment GetItem(int id);
        int Save(Comment comment);
        //bool IsExist(int id);
        int GetArticleId(int id);
        int GetReplyCommentId(int id);
      //  int GetBaseCommentDepth(int id);
        void Delete(int id);
        IList<Comment> GetList(int articleId);
        CommentInfo GetCommentInfo(int id);
    }
}
