namespace RememberText.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public string LangCode { get; set; }
    }

    public class TagWithDeleteViewModel : TagViewModel
    {
        public int TextId { get; set; }
    }

    public class TagAndTaggedTexts : TagViewModel
    {
        public int TaggedTopics { get; set; }
    }
}
