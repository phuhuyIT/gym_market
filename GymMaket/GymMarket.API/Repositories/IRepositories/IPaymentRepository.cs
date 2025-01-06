using GymMarket.API.DTOs.Payment;
using GymMarket.API.Models;

namespace GymMarket.API.Repositories.IRepositories
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetPaymentsOfCourse(string courseId);
        Task<Payment> OkPayment(string paymentId);
        Task<Payment> CancelPayment(CancelPayment model);
    }
}
