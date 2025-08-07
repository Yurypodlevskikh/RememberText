using RememberText.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RememberText.Domain.Entities
{
    public class TagAssignmentGeneral : TagAssignmentEntity { }
    public class SvTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual SvText SvText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual SvTag SvTag { get; set; }
    }

    public class EnTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual EnText EnText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual EnTag EnTag { get; set; }
    }

    public class RuTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual RuText RuText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual RuTag RuTag { get; set; }
    }

    public class FiTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual FiText FiText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual FiTag FiTag { get; set; }
    }

    public class UkTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual UkText UkText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual UkTag UkTag { get; set; }
    }

    public class EsTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual EsText EsText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual EsTag EsTag { get; set; }
    }

    public class HrTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual HrText HrText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual HrTag HrTag { get; set; }
    }

    public class DeTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual DeText DeText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual DeTag DeTag { get; set; }
    }

    public class SkTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual SkText SkText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual SkTag SkTag { get; set; }
    }

    public class SlTagAssignment : TagAssignmentEntity
    {
        [ForeignKey(nameof(TextId))]
        public virtual SlText SlText { get; set; }
        [ForeignKey(nameof(TagId))]
        public virtual SlTag SlTag { get; set; }
    }
}
