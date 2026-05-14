using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment, string>
    {
        Task<List<Payment>> GetPaymentsOfCourse(string courseId);
        Task<Payment?> OkPayment(string paymentId);
        Task<Payment?> CancelPayment(CancelPayment model);
        Task CreatePayment(Payment payment);
    }
}
