using RememberText.Domain.Entities.Base;

namespace RememberText.Models
{
    public class TagGeneral
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public int NormalizedTagId { get; set; }
        public string NormalizedTagName { get; set; }
    }

    public class TagTopicTextGeneral : TagGeneral
    {
        public string LangCode { get; set; }
        public int TopicId { get; set; }
        public int TextId { get; set; }
    }
}
