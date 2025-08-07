using RememberText.Domain.Entities.Base.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities.Base
{
    public abstract class RowVersionEntity : BaseEntity, IRowVersionEntity
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
