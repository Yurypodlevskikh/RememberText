using System;
using System.Collections.Generic;
using System.Text;

namespace RememberText.Domain.Entities.Base.Interfaces
{
    public interface ICreatedDateEntity : IBaseEntity
    {
        DateTime CreatedDateTime { get; set; }
    }
}
