using CommonDTOs;
using Flight.Airlines.Models.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceContracts.Airlines;
using ServiceContracts.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Flight.Airlines.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class DiscountTagsController : ControllerBase
    {
        private readonly CustomSettings customSettings;
        //private readonly IConfiguration config;
        private readonly IAirlinesRepository airlinesRepo;
        private readonly ILogger logger;

        public DiscountTagsController(IConfiguration config, IAirlinesRepository airlineRepo, ILogger logger)
        {
            customSettings = new CustomSettings();
            config.GetSection("CustomSettings").Bind(customSettings);
            //this.config = config;
            this.airlinesRepo = airlineRepo;
            this.logger = logger;
        }

        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "DiscountTagsController -> Pong";
        }

        [HttpGet]
        public IEnumerable<AirlinesDTOs.DiscountTags> Get()
        {
            return airlinesRepo.GetDiscountTags();
        }

        [HttpGet]
        [Route("{id}")]
        //[Route("GetDiscountTag/{id}")]
        public IEnumerable<AirlinesDTOs.DiscountTags> Get(long id)
        {
            return airlinesRepo.GetDiscountTags(id);
        }

        [HttpPost]
        [Route("GetDiscountTagsByFiltercondition")]
        public IEnumerable<AirlinesDTOs.DiscountTags> GetDiscountTagsByFiltercondition([FromBody] AirlinesDTOs.DiscountTagDetails discountTag)
        {
            return airlinesRepo.GetDiscountTagsByFiltercondition(discountTag);
        }

        [HttpPost]
        [Route("Add")]
        public long Add([FromBody] AirlinesDTOs.DiscountTags discountTag)
        {
            if (!AirlinesValidation.ValidateAddDiscountTag(discountTag))
                throw new Exception("AirlinesValidation.ValidateAddDiscountTag Falied");

            if (airlinesRepo.IsDiscountTagAlreadyExists(discountTag))
                throw new Exception("DiscountTag name and/or code already exists");

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            discountTag.Createdby = Convert.ToInt64(userId);
            discountTag.ModifiedBy = Convert.ToInt64(userId);
            return airlinesRepo.AddDiscountTag(discountTag);
        }

        [HttpPost]
        [Route("Update")]
        public Result Update([FromBody] AirlinesDTOs.DiscountTagDetails discountTag)
        {
            if (!AirlinesValidation.ValidateUpdateDiscountTag(discountTag))
                throw new Exception("AirlinesValidation.ValidateUpdateDiscountTag Falied");

            AirlinesDTOs.DiscountTags discountTag_1 = new AirlinesDTOs.DiscountTags()
            {
                Id = discountTag.Id,
                Name = discountTag.Name,
                DiscountCode = discountTag.DiscountCode,
                Description = discountTag.Description,
                Discount = discountTag.Discount
            };
            if (discountTag.IsByRate != null)
                discountTag_1.IsByRate = (bool)discountTag.IsByRate;
            if (discountTag.IsActive != null)
                discountTag_1.IsActive = (bool)discountTag.IsActive;
            if (airlinesRepo.IsDiscountTagAlreadyExists(discountTag_1))
                throw new Exception("DiscountTag name and/or code already exists");

            long userId = Convert.ToInt64(HttpContext.Request.Headers["UserId"]);
            return airlinesRepo.UpdateDiscountTag(discountTag, userId);
        }

        [HttpPost]
        [Route("ActivateDeactivateDiscountTag")]
        public Result ActivateDeactivateDiscountTag([FromBody] dynamic obj)
        {
            if (!AirlinesValidation.ValidateActivateDeactivateDiscountTag(obj.GetProperty("Id"), obj.GetProperty("IsActive")))
                throw new Exception("AirlinesValidation.ValidateActivateDeactivateDiscountTag Falied");

            AirlinesDTOs.DiscountTags discountTag = new AirlinesDTOs.DiscountTags()
            {
                Id = Convert.ToInt64(obj.GetProperty("Id").ToString()),
                IsActive = Convert.ToBoolean(obj.GetProperty("IsActive").ToString().Trim()),
                ModifiedBy = Convert.ToInt64(HttpContext?.Request?.Headers["UserId"])
            };
            return airlinesRepo.ActivateDeactivateDiscountTag(discountTag);
        }

        [HttpPost]
        [Route("Delete")]
        public Result Delete([FromBody] long id)
        {
            var discountTag = new AirlinesDTOs.DiscountTags()
            {
                Id = id,
                ModifiedBy = Convert.ToInt64(HttpContext.Request.Headers["UserId"])
            };
            return airlinesRepo.DeleteDiscountTag(discountTag);
        }

        [HttpPost]
        [Route("PermanentDelete")]
        public Result PermanentDelete([FromBody] long id)
        {
            return airlinesRepo.PermanentDeleteDiscountTag(id);
        }
    }
}
