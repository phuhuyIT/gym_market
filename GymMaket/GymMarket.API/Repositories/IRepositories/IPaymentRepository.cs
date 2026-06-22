using GymMarket.API.DTOs.Payment;
using GymMarket.API.DTOs.Response;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment, string>
    {
        Task<List<GetPaymentDto>> GetPaymentsOfCourse(string courseId);
        Task<PagedResult<GetPaymentDto>> SearchPayments(
            int pageIndex = 1,
            int pageSize = Defaults.PageSize,
            string? search = null,
            string? courseId = null,
            string? studentId = null,
            string? status = null,
            string? paymentType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? trainerId = null,
            bool includeAllCourses = false);
        Task<Payment?> OkPayment(string paymentId);
        Task<Payment?> CancelPayment(CancelPayment model);

        // Single success path used by every payment flow (manual QR approval, Momo,
        // future banking API): marks the student's payment for the course as Paid,
        // updating the existing pending record in place rather than inserting a new one,
        // and keeps the matching CourseRegistration in sync. Idempotent.
        Task<Payment?> ConfirmCoursePayment(string studentId, string courseId, string paymentType);

        // Gate for study access: true when the student has a successful payment for the course.
        Task<bool> HasPaidForCourse(string studentId, string courseId);
    }
}
