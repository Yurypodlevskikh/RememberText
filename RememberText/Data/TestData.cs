using RememberText.Domain.Entities;
using RememberText.Models;
using System.Collections.Generic;

namespace RememberText.Data
{
    public class TestData
    {
        public static List<IpAddress> IpAddresses { get; } = new List<IpAddress>
        {
            new IpAddress
            {
                Id = 1,
                Ip = "134.201.250.155",
                ContinentCode = "NA",
                ContinentName = "North America",
                CountryCode = "US",
                CountryName = "United States",
                RegionCode = "CA",
                RegionName = "California",
                City = "Los Angeles",
                Zip = "90013"
            },
            new IpAddress
            {
                Id = 2,
                Ip = "83.248.115.115",
                ContinentCode = "EU",
                ContinentName = "Europe",
                CountryCode = "SE",
                CountryName = "Sweden",
                RegionCode = "O",
                RegionName = "Västra Götaland",
                City = "Västra Frölunda",
                Zip = "42144"
            }
        };

        public static List<Language> Languages { get; } = new List<Language>
        {
            new Language
            {
                Id = 1,
                LangCode = "en-GB",
                LangName = "English - United Kingdom",
                PrimaryLang = true
            },
            new Language
            {
                Id = 2,
                LangCode = "ru-RU",
                LangName = "Russian - Russia",
                PrimaryLang = true
            },
            new Language
            {
                Id = 3,
                LangCode = "sv-SE",
                LangName = "Swedish - Sweden",
                PrimaryLang = true
            },
            new Language
            {
                Id = 4,
                LangCode = "en-AU",
                LangName = "English - Australia",
                PrimaryLang = false
            },
            new Language
            {
                Id = 5,
                LangCode = "en-CA",
                LangName = "English - Canada",
                PrimaryLang = false
            },
            new Language
            {
                Id = 6,
                LangCode = "en-CB",
                LangName = "English - Caribbean",
                PrimaryLang = false
            }
        };
    }
}
