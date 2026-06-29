using System.Security.Claims;
using GymMarket.API.Data;
using GymMarket.API.DTOs.ClassSchedule;
using GymMarket.API.Models;
using GymMarket.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClassScheduleController : ControllerBase
{
    private readonly GymMarketContext _context;
    private readonly INotificationRepository _notificationRepository;

    public ClassScheduleController(GymMarketContext context, INotificationRepository notificationRepository)
    {
        _context = context;
        _notificationRepository = notificationRepository;
    }

    [HttpGet("sessions")]
    public async Task<ActionResult<List<ClassSessionDto>>> GetSessions(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] bool includeCancelled = false)
    {
        var fromDate = (from ?? DateTime.UtcNow.Date).ToUniversalTime();
        var toDate = (to ?? fromDate.AddDays(30)).ToUniversalTime();
        var studentId = CurrentStudentId();

        var query = _context.GymClassSessions
            .Include(s => s.Trainer)
            .Include(s => s.Bookings)
            .Where(s => s.StartsAt >= fromDate && s.StartsAt <= toDate);

        if (!includeCancelled)
        {
            query = query.Where(s => s.Status != ClassSessionStatus.Cancelled);
        }

        var sessions = await query
            .OrderBy(s => s.StartsAt)
            .Take(200)
            .ToListAsync();

        return Ok(sessions.Select(s => ToSessionDto(s, studentId)).ToList());
    }

    [HttpGet("manage/sessions")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<ClassSessionDto>>> GetManageSessions([FromQuery] bool includeCancelled = true)
    {
        var query = _context.GymClassSessions
            .Include(s => s.Trainer)
            .Include(s => s.Bookings)
            .AsQueryable();

        if (User.IsInRole("Trainer"))
        {
            var trainerId = CurrentTrainerId();
            if (string.IsNullOrEmpty(trainerId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_TRAINER" });
            }

            query = query.Where(s => s.TrainerId == trainerId);
        }

        if (!includeCancelled)
        {
            query = query.Where(s => s.Status != ClassSessionStatus.Cancelled);
        }

        var sessions = await query
            .OrderByDescending(s => s.StartsAt)
            .Take(200)
            .ToListAsync();

        return Ok(sessions.Select(s => ToSessionDto(s)).ToList());
    }

    [HttpPost("sessions")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<ClassSessionDto>> CreateSession(UpsertClassSessionDto dto)
    {
        var validation = await ValidateSession(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var trainerId = await ResolveTrainerId(dto.TrainerId);
        if (trainerId == TrainerAccess.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "TRAINER_SESSION_FORBIDDEN" });
        }

        var now = DateTime.UtcNow;
        var session = new GymClassSession
        {
            ClassSessionId = Guid.NewGuid().ToString(),
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            TrainerId = trainerId,
            StartsAt = dto.StartsAt.ToUniversalTime(),
            EndsAt = dto.EndsAt.ToUniversalTime(),
            Capacity = dto.Capacity,
            Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
            Status = NormalizeSessionStatus(dto.Status),
            CreatedAt = now
        };

        _context.GymClassSessions.Add(session);
        await _context.SaveChangesAsync();

        session.Trainer = string.IsNullOrEmpty(session.TrainerId)
            ? null
            : await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == session.TrainerId);

        if (!string.IsNullOrWhiteSpace(session.Trainer?.UserId))
        {
            await _notificationRepository.NotifyUser(
                session.Trainer.UserId,
                NotificationTypes.Class,
                "Class scheduled",
                $"You have a new class: {session.Title} on {FormatDateTime(session.StartsAt)}.",
                "/agency/classes");
        }

        return Ok(ToSessionDto(session));
    }

    [HttpPut("sessions/{classSessionId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<ClassSessionDto>> UpdateSession(string classSessionId, UpsertClassSessionDto dto)
    {
        var session = await _context.GymClassSessions
            .Include(s => s.Trainer)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.ClassSessionId == classSessionId);

        if (session == null)
        {
            return NotFound(new { Message = "CLASS_SESSION_NOT_FOUND" });
        }

        if (!CanManageSession(session))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "CLASS_SESSION_FORBIDDEN" });
        }

        var validation = await ValidateSession(dto);
        if (validation != null)
        {
            return BadRequest(new { Message = validation });
        }

        var trainerId = await ResolveTrainerId(dto.TrainerId);
        if (trainerId == TrainerAccess.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "TRAINER_SESSION_FORBIDDEN" });
        }

        session.Title = dto.Title.Trim();
        session.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        session.TrainerId = trainerId;
        session.StartsAt = dto.StartsAt.ToUniversalTime();
        session.EndsAt = dto.EndsAt.ToUniversalTime();
        session.Capacity = dto.Capacity;
        session.Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim();
        session.Status = NormalizeSessionStatus(dto.Status);
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        session.Trainer = string.IsNullOrEmpty(session.TrainerId)
            ? null
            : await _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == session.TrainerId);

        return Ok(ToSessionDto(session));
    }

    [HttpDelete("sessions/{classSessionId}")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<IActionResult> CancelSession(string classSessionId)
    {
        var session = await _context.GymClassSessions
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.ClassSessionId == classSessionId);

        if (session == null)
        {
            return NotFound(new { Message = "CLASS_SESSION_NOT_FOUND" });
        }

        if (!CanManageSession(session))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "CLASS_SESSION_FORBIDDEN" });
        }

        var now = DateTime.UtcNow;
        var bookedStudentIds = session.Bookings
            .Where(b => b.Status == ClassBookingStatus.Booked)
            .Select(b => b.StudentId)
            .Distinct()
            .ToList();

        session.Status = ClassSessionStatus.Cancelled;
        session.UpdatedAt = now;

        foreach (var booking in session.Bookings.Where(b => b.Status == ClassBookingStatus.Booked))
        {
            booking.Status = ClassBookingStatus.Cancelled;
            booking.CancelledAt = now;
            booking.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        if (bookedStudentIds.Count > 0)
        {
            var studentUserIds = await _context.Students
                .Where(s => bookedStudentIds.Contains(s.StudentId) && s.UserId != null)
                .Select(s => s.UserId!)
                .ToListAsync();

            await _notificationRepository.NotifyUsers(
                studentUserIds,
                NotificationTypes.Class,
                "Class cancelled",
                $"{session.Title} on {FormatDateTime(session.StartsAt)} was cancelled.",
                "/client/classes");
        }

        return Ok(new { Message = "CLASS_SESSION_CANCELLED" });
    }

    [HttpGet("sessions/{classSessionId}/bookings")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<List<ClassBookingDto>>> GetBookings(string classSessionId)
    {
        var session = await _context.GymClassSessions.FirstOrDefaultAsync(s => s.ClassSessionId == classSessionId);
        if (session == null)
        {
            return NotFound(new { Message = "CLASS_SESSION_NOT_FOUND" });
        }

        if (!CanManageSession(session))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "CLASS_SESSION_FORBIDDEN" });
        }

        var bookings = await _context.ClassBookings
            .Include(b => b.ClassSession)
            .Include(b => b.Student)
            .Where(b => b.ClassSessionId == classSessionId)
            .OrderBy(b => b.BookedAt)
            .ToListAsync();

        return Ok(bookings.Select(ToBookingDto).ToList());
    }

    [HttpPost("sessions/{classSessionId}/book")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ClassBookingDto>> BookSession(string classSessionId)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        if (!await HasActiveMembership(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "ACTIVE_MEMBERSHIP_REQUIRED" });
        }

        var session = await _context.GymClassSessions
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.ClassSessionId == classSessionId);

        if (session == null)
        {
            return NotFound(new { Message = "CLASS_SESSION_NOT_FOUND" });
        }

        if (session.Status != ClassSessionStatus.Scheduled)
        {
            return Conflict(new { Message = "CLASS_SESSION_NOT_BOOKABLE" });
        }

        if (session.StartsAt <= DateTime.UtcNow)
        {
            return Conflict(new { Message = "CLASS_SESSION_ALREADY_STARTED" });
        }

        var existing = session.Bookings.FirstOrDefault(b =>
            b.StudentId == studentId
            && (b.Status == ClassBookingStatus.Booked || b.Status == ClassBookingStatus.Attended));
        if (existing != null)
        {
            return Conflict(new { Message = "CLASS_ALREADY_BOOKED" });
        }

        if (ActiveBookingCount(session) >= session.Capacity)
        {
            return Conflict(new { Message = "CLASS_FULL" });
        }

        var booking = new ClassBooking
        {
            BookingId = Guid.NewGuid().ToString(),
            ClassSessionId = session.ClassSessionId,
            StudentId = studentId,
            Status = ClassBookingStatus.Booked,
            BookedAt = DateTime.UtcNow
        };

        _context.ClassBookings.Add(booking);
        await _context.SaveChangesAsync();

        booking.ClassSession = session;
        booking.Student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (!string.IsNullOrWhiteSpace(booking.Student?.UserId))
        {
            await _notificationRepository.NotifyUser(
                booking.Student.UserId,
                NotificationTypes.Class,
                "Class booked",
                $"You're booked for {session.Title} on {FormatDateTime(session.StartsAt)}.",
                "/client/classes");
        }

        await NotifyTrainerOfClassActivity(
            session.TrainerId,
            "New class booking",
            $"{booking.Student?.Name ?? "A student"} booked {session.Title}.",
            "/agency/classes");

        return Ok(ToBookingDto(booking));
    }

    [HttpPost("bookings/{bookingId}/cancel")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CancelBooking(string bookingId)
    {
        var studentId = CurrentStudentId();
        if (string.IsNullOrEmpty(studentId))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "NOT_A_STUDENT" });
        }

        var booking = await _context.ClassBookings
            .Include(b => b.ClassSession)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.StudentId == studentId);

        if (booking == null)
        {
            return NotFound(new { Message = "CLASS_BOOKING_NOT_FOUND" });
        }

        if (booking.Status != ClassBookingStatus.Booked)
        {
            return Conflict(new { Message = "CLASS_BOOKING_NOT_CANCELLABLE" });
        }

        var now = DateTime.UtcNow;
        booking.Status = ClassBookingStatus.Cancelled;
        booking.CancelledAt = now;
        booking.UpdatedAt = now;
        await _context.SaveChangesAsync();

        await NotifyStudent(
            studentId,
            NotificationTypes.Class,
            "Booking cancelled",
            $"Your booking for {booking.ClassSession?.Title ?? "a class"} was cancelled.",
            "/client/classes");

        await NotifyTrainerOfClassActivity(
            booking.ClassSession?.TrainerId,
            "Class booking cancelled",
            $"A student cancelled their booking for {booking.ClassSession?.Title ?? "a class"}.",
            "/agency/classes");

        return Ok(new { Message = "CLASS_BOOKING_CANCELLED" });
    }

    [HttpPost("bookings/{bookingId}/attendance")]
    [Authorize(Roles = "Trainer,Admin")]
    public async Task<ActionResult<ClassBookingDto>> MarkAttendance(string bookingId, MarkClassAttendanceDto dto)
    {
        var status = NormalizeAttendanceStatus(dto.Status);
        if (status == null)
        {
            return BadRequest(new { Message = "CLASS_ATTENDANCE_STATUS_INVALID" });
        }

        var booking = await _context.ClassBookings
            .Include(b => b.ClassSession)
            .Include(b => b.Student)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking == null)
        {
            return NotFound(new { Message = "CLASS_BOOKING_NOT_FOUND" });
        }

        if (booking.ClassSession == null || !CanManageSession(booking.ClassSession))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { Message = "CLASS_SESSION_FORBIDDEN" });
        }

        if (booking.Status == ClassBookingStatus.Cancelled)
        {
            return Conflict(new { Message = "CLASS_BOOKING_CANCELLED" });
        }

        var now = DateTime.UtcNow;
        booking.Status = status;
        booking.AttendanceMarkedAt = now;
        booking.UpdatedAt = now;
        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(booking.Student?.UserId))
        {
            var title = status == ClassBookingStatus.Attended ? "Attendance marked" : "Marked as no-show";
            await _notificationRepository.NotifyUser(
                booking.Student.UserId,
                NotificationTypes.Class,
                title,
                $"{booking.ClassSession?.Title ?? "Your class"} was marked as {status}.",
                "/client/classes");
        }

        return Ok(ToBookingDto(booking));
    }

    private async Task<string?> ValidateSession(UpsertClassSessionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return "CLASS_TITLE_REQUIRED";
        }

        if (dto.Capacity <= 0)
        {
            return "CLASS_CAPACITY_INVALID";
        }

        if (dto.EndsAt.ToUniversalTime() <= dto.StartsAt.ToUniversalTime())
        {
            return "CLASS_TIME_RANGE_INVALID";
        }

        var status = NormalizeSessionStatus(dto.Status);
        if (status != ClassSessionStatus.Scheduled
            && status != ClassSessionStatus.Cancelled
            && status != ClassSessionStatus.Completed)
        {
            return "CLASS_STATUS_INVALID";
        }

        if (!string.IsNullOrWhiteSpace(dto.TrainerId)
            && !await _context.Trainers.AnyAsync(t => t.TrainerId == dto.TrainerId.Trim()))
        {
            return "TRAINER_NOT_FOUND";
        }

        return null;
    }

    private async Task<string?> ResolveTrainerId(string? requestedTrainerId)
    {
        if (User.IsInRole("Trainer"))
        {
            var currentTrainerId = CurrentTrainerId();
            if (string.IsNullOrEmpty(currentTrainerId))
            {
                return TrainerAccess.Forbidden;
            }

            if (!string.IsNullOrWhiteSpace(requestedTrainerId) && requestedTrainerId.Trim() != currentTrainerId)
            {
                return TrainerAccess.Forbidden;
            }

            return currentTrainerId;
        }

        var trainerId = string.IsNullOrWhiteSpace(requestedTrainerId) ? null : requestedTrainerId.Trim();
        if (!string.IsNullOrEmpty(trainerId) && !await _context.Trainers.AnyAsync(t => t.TrainerId == trainerId))
        {
            return TrainerAccess.Forbidden;
        }

        return trainerId;
    }

    private bool CanManageSession(GymClassSession session)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        var trainerId = CurrentTrainerId();
        return !string.IsNullOrEmpty(trainerId) && session.TrainerId == trainerId;
    }

    private async Task<bool> HasActiveMembership(string studentId)
    {
        var now = DateTime.UtcNow;
        return await _context.StudentMemberships.AnyAsync(m =>
            m.StudentId == studentId
            && m.Status == MembershipStatus.Active
            && m.StartsAt <= now
            && m.EndsAt >= now);
    }

    private static ClassSessionDto ToSessionDto(GymClassSession session, string? studentId = null)
    {
        var bookedCount = ActiveBookingCount(session);
        var myBooking = string.IsNullOrEmpty(studentId)
            ? null
            : session.Bookings
                .Where(b => b.StudentId == studentId)
                .OrderByDescending(b => b.BookedAt)
                .FirstOrDefault();

        return new ClassSessionDto
        {
            ClassSessionId = session.ClassSessionId,
            Title = session.Title,
            Description = session.Description,
            TrainerId = session.TrainerId,
            TrainerName = session.Trainer?.Name,
            StartsAt = session.StartsAt,
            EndsAt = session.EndsAt,
            Capacity = session.Capacity,
            BookedCount = bookedCount,
            AvailableSpots = Math.Max(0, session.Capacity - bookedCount),
            Location = session.Location,
            Status = session.Status,
            IsBooked = myBooking?.Status == ClassBookingStatus.Booked || myBooking?.Status == ClassBookingStatus.Attended,
            MyBookingId = myBooking?.BookingId,
            MyBookingStatus = myBooking?.Status
        };
    }

    private static ClassBookingDto ToBookingDto(ClassBooking booking)
    {
        return new ClassBookingDto
        {
            BookingId = booking.BookingId,
            ClassSessionId = booking.ClassSessionId,
            ClassTitle = booking.ClassSession?.Title ?? string.Empty,
            StudentId = booking.StudentId,
            StudentName = booking.Student?.Name,
            StudentEmail = booking.Student?.Email,
            Status = booking.Status,
            BookedAt = booking.BookedAt,
            CancelledAt = booking.CancelledAt,
            AttendanceMarkedAt = booking.AttendanceMarkedAt
        };
    }

    private static int ActiveBookingCount(GymClassSession session)
    {
        return session.Bookings.Count(b =>
            b.Status == ClassBookingStatus.Booked
            || b.Status == ClassBookingStatus.Attended);
    }

    private static string NormalizeSessionStatus(string? status)
    {
        return status?.Trim() switch
        {
            ClassSessionStatus.Cancelled => ClassSessionStatus.Cancelled,
            ClassSessionStatus.Completed => ClassSessionStatus.Completed,
            _ => ClassSessionStatus.Scheduled
        };
    }

    private static string? NormalizeAttendanceStatus(string? status)
    {
        return status?.Trim() switch
        {
            ClassBookingStatus.Attended => ClassBookingStatus.Attended,
            ClassBookingStatus.NoShow => ClassBookingStatus.NoShow,
            _ => null
        };
    }

    private async Task NotifyStudent(string studentId, string type, string title, string content, string link)
    {
        var userId = await _context.Students
            .Where(s => s.StudentId == studentId)
            .Select(s => s.UserId)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await _notificationRepository.NotifyUser(userId, type, title, content, link);
        }
    }

    private async Task NotifyTrainerOfClassActivity(string? trainerId, string title, string content, string link)
    {
        if (string.IsNullOrWhiteSpace(trainerId))
        {
            return;
        }

        var userId = await _context.Trainers
            .Where(t => t.TrainerId == trainerId)
            .Select(t => t.UserId)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await _notificationRepository.NotifyUser(userId, NotificationTypes.Class, title, content, link);
        }
    }

    private static string FormatDateTime(DateTime dateTime)
    {
        return $"{dateTime:MMM d, yyyy h:mm tt} UTC";
    }

    private string CurrentStudentId()
    {
        return User.FindFirstValue("studentId") ?? string.Empty;
    }

    private string CurrentTrainerId()
    {
        return User.FindFirstValue("trainerId") ?? string.Empty;
    }

    private static class TrainerAccess
    {
        public const string Forbidden = "__FORBIDDEN__";
    }
}
