using RememberText.Domain.Entities.Base;
using RememberText.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RememberText.Domain.Entities
{
    public class IpAddress : CreatedDateEntity
    {
        [Required]
        [StringLength(15)]
        public string Ip { get; set; }
        [StringLength(256)]
        public string HostName { get; set; }
        [StringLength(256)]
        public string Timezone { get; set; }
        public string Loc { get; set; }
        [StringLength(10)]
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string Organization { get; set; }
        public string City { get; set; }
        [StringLength(10)]
        public string Zip { get; set; }
        [StringLength(256)]
        public string Browser { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
    }

    public class Visit
    {
        [Required]
        public DateTime Visited { get; set; }
        [Required]
        public int IpId { get; set; }
        [ForeignKey(nameof(IpId))]
        public virtual IpAddress IpAddress { get; set; }
    }
}
