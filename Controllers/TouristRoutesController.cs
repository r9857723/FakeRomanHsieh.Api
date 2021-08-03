using System;
using System.Linq;
using FakeRomanHsieh.API.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FakeRomanHsieh.API.ResourceParameters;
using Microsoft.AspNetCore.JsonPatch;
using FakeRomanHsieh.API.Helper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Microsoft.Net.Http.Headers;
using System.Dynamic;

namespace FakeRomanHsieh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _touristRoutesRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IPropertyMappingService propertyMappingService)
        {
            _touristRoutesRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;  
        }

        private String GenerateTouristRouteResourceURL(TouristRouteResourceParameters parameters, PaginationResourceParameters parameters2, ResourceUrlType resourceUrlType)
        {
            return resourceUrlType switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link(
                    "GetTouistRoutes",
                    new
                    {
                        keyWord = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber - 1,
                        pageSize = parameters2.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }
                ),
                ResourceUrlType.NextPage => _urlHelper.Link(
                    "GetTouistRoutes",
                    new
                    {
                        keyWord = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber + 1,
                        pageSize = parameters2.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }
                ),
                _ => _urlHelper.Link(
                    "GetTouistRoutes",
                    new
                    {
                        keyWord = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters2.PageNumber,
                        pageSize = parameters2.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }
                ),
            };
        }

        /// <summary>
        /// 查詢所有旅遊路徑
        /// application/json
        /// application/vnd.Roman.hateoas+json
        /// application/vnd.Roman.tourisrRoute.simplity+json 簡化版
        /// application/vnd.Roman.tourisrRoute.simplity.hateoas+json 簡化版+超媒體數據
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Produces(
            "application/json",
            "application/vnd.Roman.hateoas+json",
            "application/vnd.Roman.tourisrRoute.simplify+json",
            "application/vnd.Roman.tourisrRoute.simplify.hateoas+json"
            )]
        [HttpGet(Name = "GetTouistRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouistRoutes([FromQuery] TouristRouteResourceParameters parameters, [FromQuery] PaginationResourceParameters parameters2, [FromHeader(Name = "Accept")] String mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parseMediaType))
            {
                return BadRequest();
            }     

            if (!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy))
            {
                return BadRequest("請輸入正確排序參數");
            }
            if (!_propertyMappingService.IsPropertyExists<TouristRouteDto>(parameters.Fields))
            {
                return BadRequest("請輸入正確塑形參數");
            }


            var touristRouteFromRepo = await _touristRoutesRepository.GetTouristRoutesAsync(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue, parameters2.PageNumber, parameters2.PageSize, parameters.OrderBy);
            if (touristRouteFromRepo == null || touristRouteFromRepo.Count() <= 0)
            {
                return NotFound("找不到旅遊路線");
            }

            var previousPageLink = touristRouteFromRepo.HasPrevious
                ? GenerateTouristRouteResourceURL(parameters, parameters2, ResourceUrlType.PreviousPage)
                : null;
            var nextPageLink = touristRouteFromRepo.HasNext
                ? GenerateTouristRouteResourceURL(parameters, parameters2, ResourceUrlType.NextPage)
                : null;

            var paginationMetaData = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRouteFromRepo.TotalCount,
                totalPage = touristRouteFromRepo.TotalPage,
                currentPage = touristRouteFromRepo.CurrentPage,
                pageSize = touristRouteFromRepo.PageSize
            };

            Response.Headers.Add("x-pagination", JsonConvert.SerializeObject(paginationMetaData));

            bool isHateoas = parseMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            var primryMediaType = isHateoas ? parseMediaType.SubTypeWithoutSuffix.Substring(0, parseMediaType.SubTypeWithoutSuffix.Length - 8) : parseMediaType.SubTypeWithoutSuffix;


            //var touristRouteDtos = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRouteFromRepo);
            //var shapedDtoList = touristRouteDtos.ShapeData(parameters.Fields);

            IEnumerable<object> touristRouteDto;
            IEnumerable<ExpandoObject> shapedDtoList;

            if (primryMediaType == "vnd.Roman.tourisrRoute.simplify")
            {
                touristRouteDto = _mapper.Map<IEnumerable<TouristRouteSimplifyDto>>(touristRouteFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteSimplifyDto>)touristRouteDto).ShapeData(parameters.Fields);
            } else
            {
                touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRouteFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteDto>)touristRouteDto).ShapeData(parameters.Fields);
            }

            if (isHateoas)
            {
                var linkDto = CreateLinksFOrTouristRouteList(parameters, parameters2);

                var shapedDtoWithLinkList = shapedDtoList.Select(x =>
                {
                    var touristRouteDictionary = x as IDictionary<String, object>;
                    var links = CreateLinkForTouristRoute((Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("Links", links);
                    return touristRouteDictionary;
                });

                var result = new
                {
                    value = shapedDtoWithLinkList,
                    links = linkDto
                };

                return Ok(result);
            }
            return Ok(shapedDtoList);
        }

        private IEnumerable<LinkDto> CreateLinksFOrTouristRouteList(TouristRouteResourceParameters parameters, PaginationResourceParameters parameters2)
        {
            var links = new List<LinkDto>();

            // 自我連接
            links.Add(
                new LinkDto(
                    GenerateTouristRouteResourceURL(parameters, parameters2, ResourceUrlType.CurrentPage),
                    "self",
                    "GET"
                )
            );

            // 創建旅遊路線
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_toursit_route",
                    "POST"
                )
            );


            return links;
        }

        /// <summary>
        /// 查詢旅遊路徑
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpGet("{touristRouteId}", Name = "GetTouistRouteById")]
        public async Task<IActionResult> GetTouistRouteById(Guid touristRouteId, string fields)
        {
            var touristRouteFromRepo = await _touristRoutesRepository.GetTouristRouteAsync(touristRouteId);

            if (touristRouteFromRepo == null)
            {
                return NotFound($"找不到旅遊路線{touristRouteId}");
            }
            TouristRouteDto touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            //return Ok(touristRouteDto.ShapeData(fields));
            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);
            var result = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            result.Add("Links", linkDtos);

            return Ok(result);
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRoute(Guid touristRouteId, string fields)
        {
            var links = new List<LinkDto>();
            //self
            links.Add(
                new LinkDto(
                    Url.Link("GetTouistRouteById", new { touristRouteId, fields }),
                    "self",
                    "GET"
                )
            );
            // update
            links.Add(
                new LinkDto(
                    Url.Link("UpdateTouristRoute", new { touristRouteId }),
                    "update",
                    "PUT"
                )
            );
            // patch
            links.Add(
                new LinkDto(
                    Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                    "partially_update",
                    "PATCH"
                )
            );
            // delete
            links.Add(
                new LinkDto(
                    Url.Link("DeleteTouristRoute", new { touristRouteId }),
                    "delete",
                    "DELETE"
                )
            );
            // 旅遊路線圖片
            links.Add(
                new LinkDto(
                    Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                    "get_picture",
                    "GET"
                )
            );

            // 新增旅遊路線圖片
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                    "create_picture",
                    "POST"
                )
            );
            return links;
        }

        /// <summary>
        /// 新增旅遊路徑
        /// </summary>
        /// <param name="touristRouteForCreationDto"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto) {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRoutesRepository.AddTouristRoute(touristRouteModel);
            await _touristRoutesRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);
            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;
            result.Add("Links", links);

            return CreatedAtRoute(
                "GetTouistRouteById",
                new { touristRouteId = result["Id"] },
                result
                );
        }

        /// <summary>
        /// 更新旅遊路徑（Put）
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRouteForUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("{touristRouteId}", Name = "UpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute] Guid touristRouteId, [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線找不到");
            }
            TouristRoute touristRouteFromRepo = await _touristRoutesRepository.GetTouristRouteAsync(touristRouteId);
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            await _touristRoutesRepository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 更新旅遊路徑（Patch）
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute([FromRoute] Guid touristRouteId, [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線找不到");
            }
            TouristRoute touristRouteFromRepo = await _touristRoutesRepository.GetTouristRouteAsync(touristRouteId);
            TouristRouteForUpdateDto toursitRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(toursitRouteToPatch, ModelState);
            if (!TryValidateModel(toursitRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(toursitRouteToPatch, touristRouteFromRepo);
            await _touristRoutesRepository.SaveAsync();

            return NoContent();
        }


        /// <summary>
        /// 刪除旅遊路線
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute(Guid touristRouteId)
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線找不到");
            }
            TouristRoute touristRoute = await _touristRoutesRepository.GetTouristRouteAsync(touristRouteId);
            _touristRoutesRepository.DeleteTouristRoute(touristRoute);
            await _touristRoutesRepository.SaveAsync();

            return NoContent();

        }

        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if (touristIDs == null)
            {
                return BadRequest();
            }
            IEnumerable<TouristRoute> touristRoutes = await _touristRoutesRepository.GetTouristRouteByIdsAsync(touristIDs);
            _touristRoutesRepository.DeleteTouristRoutes(touristRoutes);
            await _touristRoutesRepository.SaveAsync();
            return NoContent();
        }
    }
}
