using RememberText.Domain.Entities.Base.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RememberText.Domain.Entities.Base
{
    public abstract class TagEntity : BaseEntity, ITagEntity
    {
        [Required]
        [StringLength(256)]
        public string TagName { get; set; }
        [Required]
        public int NormalizedTagId { get; set; }
        [ForeignKey(nameof(NormalizedTagId))]
        public virtual NormalizedTag NormalizedTag {get;set;}
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
