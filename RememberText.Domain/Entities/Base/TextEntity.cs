using RememberText.Domain.Entities.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RememberText.Domain.Entities.Base
{
    public abstract class TextEntity : BaseEntity, ITextEntity
    {
        [Required]
        public string TextContent { get; set; }
        [Required]
        public string AuthorId {  get; set; }
        public string AuthorNickname { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string Redactor { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int TopicId { get; set; }
        [ForeignKey(nameof(TopicId))]
        public virtual Topic Topic { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public virtual Language Language { get; set; }
    }
}
