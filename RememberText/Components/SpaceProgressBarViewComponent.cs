using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RememberText.Domain.Entities;
using RememberText.Domain.Entities.Identity;
using RememberText.Infrastructure.Helpers;
using RememberText.Infrastructure.Interfaces;
using RememberText.RTTools;
using RememberText.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.Components
{
    public class SpaceProgressBarViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly IRTTopicService _topicService;
        private readonly IRTTextService _textService;
        private readonly IConfiguration _config;

        public SpaceProgressBarViewComponent(
            UserManager<User> userManager, 
            IRTTopicService topicService,
            IConfiguration textSettings,
            IRTTextService textService,
            IConfiguration config)
        {
            _userManager = userManager;
            _topicService = topicService;
            _textService = textService;
            _config = config;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await CalculateAvailableSpace());

        private async Task<SpaceProgressBarViewModel> CalculateAvailableSpace()
        {
            User user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

            if(user.LimitOfText > 0)
            {
                IEnumerable<TopicsViewModel> topics = await _topicService.GetAllTopicsByLoggedInUser(user.Id);

                int amountOfText = await _textService.CalculateUserTopicsTextContentAsync(topics);
                
                int warningMin = Convert.ToInt32(_config["UserTextSettings:WarningMin"]); // 70
                int dangerMin = Convert.ToInt32(_config["UserTextSettings:DangerMin"]); // 90
                int warningMax = dangerMin - warningMin; // 20
                int dangerMax = 100 - dangerMin; // 10
                int percent = Convert.ToInt32(((decimal)amountOfText / (decimal)user.LimitOfText) * 100);
                int successPercent = percent > warningMin ? warningMin : percent;
                int warningPercent = percent > warningMin && percent < dangerMin ? percent - warningMin :
                    percent > dangerMin ? dangerMin - warningMin : 0;
                int dangerPercent = percent > dangerMin ? percent - dangerMin : 0;

                SpaceProgressBarViewModel spaceProgress = new SpaceProgressBarViewModel
                {
                    AmountOfText = amountOfText,
                    LimitationOfChars = user.LimitOfText,
                    SuccessPercent = successPercent,
                    WarningPercent = warningPercent,
                    DangerPercent = dangerPercent,
                    SuccessMax = warningMin,
                    WarningMax = warningMax,
                    DangerMax = dangerMax,
                    Percent = (int)percent
                };

                return spaceProgress;
            }

            return null;
        }
    }
}
