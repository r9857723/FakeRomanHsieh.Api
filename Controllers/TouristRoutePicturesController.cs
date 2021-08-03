using System;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FakeRomanHsieh.API.Services;
using System.Collections.Generic;
using FakeRomanHsieh.API.Models;
using System.Linq;
using FakeRomanHsieh.API.Dtos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FakeRomanHsieh.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController: ControllerBase
    {
        private ITouristRouteRepository _touristRoutesRepository;
        private readonly IMapper _mapper;
        public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRoutesRepository = touristRouteRepository ?? throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 查詢旅遊路徑的所有圖片
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetPictureListForTouristRoute")]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }

            IEnumerable<TouristRoutePicture> picturesFromRepo = await _touristRoutesRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
            if (picturesFromRepo == null || picturesFromRepo.Count() <= 0)
            {
                return NotFound("照片不存在");
            }
            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        /// <summary>
        /// 查詢旅遊路徑的圖片
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRoutePictureId"></param>
        /// <returns></returns>
        [HttpGet("{touristRoutePictureId}", Name = "GetTouristRoutePicture")]
        public async Task<IActionResult> GetTouristRoutePicture(Guid touristRouteId, int touristRoutePictureId)
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }
            TouristRoutePicture touristRoutePictureFromRepo = await _touristRoutesRepository.GetTouristRoutePictureAsync(touristRoutePictureId);
            if (touristRoutePictureFromRepo == null)
            {
                return NotFound("圖片不存在");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(touristRoutePictureFromRepo));
        }

        /// <summary>
        /// 新增旅遊路徑的圖片
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRoutePictureForCreationDto"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateTouristRoutePicture")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto
            )
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }
            TouristRoutePicture touristRoutePictureModel  = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRoutesRepository.AddTouristRoutePicture(touristRouteId, touristRoutePictureModel);
            await _touristRoutesRepository.SaveAsync();
            TouristRoutePictureDto touristRoutePictureDtoToReturn = _mapper.Map<TouristRoutePictureDto>(touristRoutePictureModel);
            return CreatedAtRoute(
                    "GetTouristRoutePicture",
                    new
                    {
                        touristRouteId = touristRoutePictureModel.TouristRouteId,
                        touristRoutePictureId = touristRoutePictureModel.Id
                    },
                    touristRoutePictureDtoToReturn
                );

        }

        /// <summary>
        /// 刪除旅遊路線圖片
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{touristRoutePictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoutePicture([FromRoute]Guid touristRouteId, [FromRoute] int touristRoutePictureId )
        {
            if (!(await _touristRoutesRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("旅遊路線不存在");
            }
            var touristRoutePicture = await _touristRoutesRepository.GetTouristRoutePictureAsync(touristRoutePictureId);
            _touristRoutesRepository.DeleteTouristRoutePicture(touristRoutePicture);
            await _touristRoutesRepository.SaveAsync();
            return NoContent();
        }
    }
}
