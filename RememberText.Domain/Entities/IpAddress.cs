using RememberText.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace RememberText.Domain.Entities
{
    public class IpAddress : CreatedDateEntity
    {
        [Required]
        public string Ip { get; set; }
        public string ContinentCode { get; set; }
        public string ContinentName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        [StringLength(450)]
        public string UserId { get; set; }
    }
}
