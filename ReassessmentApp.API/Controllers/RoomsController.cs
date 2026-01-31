using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReassessmentApp.Application.Common;
using ReassessmentApp.Application.DTOs;
using ReassessmentApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReassessmentApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoomDto>>>> GetAll()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return Ok(ApiResponse<IEnumerable<RoomDto>>.SuccessResponse(rooms));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoomDto>>> GetById(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(id);
                if (room == null)
                    return NotFound(ApiResponse<RoomDto>.FailureResponse(StatusCodes.Status404NotFound, "Room not found"));

                return Ok(ApiResponse<RoomDto>.SuccessResponse(room));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create(CreateRoomDto roomDto)
        {
            try
            {
                var id = await _roomService.CreateRoomAsync(roomDto);
                return CreatedAtAction(nameof(GetById), new { id }, 
                    ApiResponse<object>.SuccessResponse(new { id }, StatusCodes.Status201Created, "Room created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                await _roomService.DeleteRoomAsync(id);
                return Ok(ApiResponse<string>.SuccessResponse("Deleted", StatusCodes.Status200OK, "Room deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
