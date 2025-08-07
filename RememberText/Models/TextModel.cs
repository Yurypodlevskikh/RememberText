using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Models
{
    public class TextModel
    {
        public int Id { get; set; }
        public string TextContent { get; set; }
        public string AuthorId { get; set; }
        public string AuthorNickname { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string Redactor { get; set; }
        public int LanguageId { get; set; }
        public int TopicId { get; set; }
    }

    public class TextContentWithId
    {
        public int Id { get; set; }
        public string TextContent { get; set; }
    }
}
