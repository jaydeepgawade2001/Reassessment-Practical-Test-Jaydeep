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
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetAll()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(ApiResponse<IEnumerable<BookingDto>>.SuccessResponse(bookings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all bookings");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetByRoom(int roomId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByRoomAsync(roomId);
                return Ok(ApiResponse<IEnumerable<BookingDto>>.SuccessResponse(bookings));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings for room {RoomId}", roomId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BookingDto>>> GetById(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound(ApiResponse<BookingDto>.FailureResponse(StatusCodes.Status404NotFound, "Booking not found"));

                return Ok(ApiResponse<BookingDto>.SuccessResponse(booking));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking {BookingId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create(CreateBookingDto bookingDto)
        {
            try
            {
                var id = await _bookingService.CreateBookingAsync(bookingDto);
                return CreatedAtAction(nameof(GetById), new { id },
                    ApiResponse<object>.SuccessResponse(new { id }, StatusCodes.Status201Created, "Booking created successfully"));
            }
            catch (ArgumentException ex) // Validation/Business Rule
            {
                return BadRequest(ApiResponse<string>.FailureResponse(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (InvalidOperationException ex) // Conflict
            {
                return Conflict(ApiResponse<string>.FailureResponse(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Cancel(int id)
        {
            try
            {
                await _bookingService.CancelBookingAsync(id);
                return Ok(ApiResponse<string>.SuccessResponse("Cancelled", StatusCodes.Status200OK, "Booking cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
