using RememberText.Infrastructure.Helpers;

namespace RememberText.ViewModels
{
    public class TopicsAndParamViewModel
    {
        public TopicsSortParamViewModel TopicSortParam { get; set; }
        public PaginatedTopics Topics { get; set; }
    }

    public class PaginatedTopics : TopicFiltersViewModel
    {
        public PaginatedList<TopicsWithLangFlagsViewModel> PLTopics { get; set; }
    }
}
