using System;
using System.Collections.Generic;
using System.Text;

namespace RememberText.Domain.Entities.Base.Interfaces
{
    public interface ITextEntity : ICreatedDateEntity
    {
        string TextContent { get; set; }
        string AuthorId { get; set; }
        string AuthorNickname { get; set; }
        string AuthorEmail { get; set; }
        DateTime? UpdateDateTime { get; set; }
        string Redactor { get; set; }
        int LanguageId { get; set; }
        int TopicId { get; set; }
    }
}
