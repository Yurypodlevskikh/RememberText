using RememberText.Domain.Entities.Base;
using System.Collections.Generic;

namespace RememberText.Domain.Entities
{
    public class SvTag : TagEntity 
    {
        public ICollection<SvTagAssignment> SvTagAssignments { get; set; }
    }
    public class RuTag : TagEntity 
    {
        public ICollection<RuTagAssignment> RuTagAssignments { get; set; }
    }
    public class EnTag : TagEntity 
    {
        public ICollection<EnTagAssignment> EnTagAssignments { get; set; }
    }
    public class FiTag : TagEntity
    {
        public ICollection<FiTagAssignment> FiTagAssignments { get; set; }
    }
    public class UkTag : TagEntity
    {
        public ICollection<UkTagAssignment> UkTagAssignments { get; set; }
    }
    public class EsTag : TagEntity
    {
        public ICollection<EsTagAssignment> EsTagAssignments { get; set; }
    }
    public class HrTag : TagEntity
    {
        public ICollection<HrTagAssignment> HrTagAssignments { get; set; }
    }
    public class DeTag : TagEntity
    {
        public ICollection<DeTagAssignment> DeTagAssignments { get; set; }
    }
    public class SkTag : TagEntity 
    {
    
    }
    public class SlTag : TagEntity
    {

    }
}
