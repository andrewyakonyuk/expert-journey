using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebSite.Models.Repository
{
    public interface ITagRepository
    {
        int Save(Tag tag);
        Tag GetTagById(int id);
        Tag GetTagByName(string name);
        IList<Tag> GetAllTags();
        IList<Tag> SaveTagsGroup(string[] tags);
        IList<Tag> GatTagsGroupByNames(string[] tags);
    }
}
