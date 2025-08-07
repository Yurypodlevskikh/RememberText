using RememberText.Domain.Entities;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Mapping
{
    public static class TagMapping
    {
        public static TagWithDeleteViewModel ToTagWithDeleteView(TagViewModel tag, int textId) => new TagWithDeleteViewModel
        {
            Id = tag.Id,
            LangCode = tag.LangCode,
            TagName = tag.TagName,
            TextId = textId
        };
        public static List<TagWithDeleteViewModel> ToTagWithDeleteView(IEnumerable<TagViewModel> tag, int textId)
        {
            var tagViewModels = new List<TagWithDeleteViewModel>();
            foreach(var t in tag)
            {
                TagWithDeleteViewModel tagViewModel = ToTagWithDeleteView(t, textId);
                tagViewModels.Add(tagViewModel);
            }
            return tagViewModels;
        }

        public static TagAssignmentGeneral TagAssignToGeneral(dynamic obj) => new TagAssignmentGeneral
        {
            TagId = obj.TagId,
            TextId = obj.TextId
        };
        public static IEnumerable<TagAssignmentGeneral> TagAssignToGeneral(IEnumerable<dynamic> obj) => obj.Select(TagAssignToGeneral);
    }
}
