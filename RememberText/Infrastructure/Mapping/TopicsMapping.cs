using Microsoft.AspNetCore.Identity;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Infrastructure.Mapping
{
    public static class TopicsMapping
    {
        public static IEnumerable<TopicsViewModel> TopicsToUser(this IEnumerable<Topic> t, UserManager<User> userManager)
        {
            var topicsToView = new List<TopicsViewModel>();

            if(t != null && t.Count() > 0)
            {
                foreach (var topic in t)
                {
                    var user = userManager.FindByIdAsync(topic.UserId);
                    var topicToView = new TopicsViewModel
                    {
                        TopicId = topic.Id,
                        TopicTitle = topic.TopicTitle,
                        SourceLang = topic.SourceLang,
                        TargetLang = topic.TargetLang,
                        CreatedDateTime = topic.CreatedDateTime,
                        UpdatedDateTime = topic.UpdatedDateTime,
                        PublicText = topic.PublicText,
                        BanProject = topic.BanText,
                        AuthorNickname = user.Result.Nickname
                    };
                    topicsToView.Add(topicToView);
                }
            }

            return topicsToView;
        }
    }
}
