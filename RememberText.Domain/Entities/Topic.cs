using RememberText.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities
{
    public class Topic : CreatedDateEntity
    {
        [Required]
        [StringLength(256)]
        public string TopicTitle { get; set; }
        [Required]
        [StringLength(50)]
        public string Primary { get; set; }
        [StringLength(50)]
        public string Secondary { get; set; }
        public bool Public { get; set; }
        [Required]
        [StringLength(256)]
        public string UserId { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool BanText { get; set; }
        //public virtual ICollection<TextEntity> TextEntities { get; set; }
    }
}
