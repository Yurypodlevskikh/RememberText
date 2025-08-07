using RememberText.Domain.Entities.Base;
using RememberText.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RememberText.Domain.Entities
{
    public class Topic : CreatedDateEntity
    {
        [Required]
        [StringLength(256)]
        public string TopicTitle { get; set; }
        [Required]
        [StringLength(50)]
        public string SourceLang { get; set; }
        [StringLength(50)]
        public string TargetLang { get; set; }
        public bool? PublicText { get; set; }
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool BanText { get; set; }
        public int AgeLimitation { get; set; }
        public int? CopyrightId { get; set; }
        public int SourceLangId { get; set; }
        public int? TargetLangId { get; set; }
        [ForeignKey(nameof(SourceLangId))]
        public virtual Language SourceLanguage { get; set; }
        [ForeignKey(nameof(TargetLangId))]
        public virtual Language TargetLanguage { get; set; }
        [ForeignKey(nameof(CopyrightId))]
        public virtual TextCopyrightModel TextCopyright { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual ICollection<EnText> EnTexts { get; set; }
        public virtual ICollection<SvText> SvTexts { get; set; }
        public virtual ICollection<RuText> RuTexts { get; set; }
        public virtual ICollection<FiText> FiTexts { get; set; }
        public virtual ICollection<UkText> UkTexts { get; set; }
        public virtual ICollection<EsText> EsTexts { get; set; }
        public virtual ICollection<HrText> HrTexts { get; set; }
        public virtual ICollection<DeText> DeTexts { get; set; }
        public virtual ICollection<SkText> SkTexts { get; set; }
        public virtual ICollection<SlText> SlTexts { get; set; }
    }
}
