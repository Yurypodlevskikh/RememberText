using RememberText.Domain.Entities.Base.Interfaces;
using System;

namespace RememberText.Domain.Entities.Base
{
    public abstract class CreatedDateEntity : BaseEntity, ICreatedDateEntity
    {
        public DateTime CreatedDateTime { get; set; }
    }
}
