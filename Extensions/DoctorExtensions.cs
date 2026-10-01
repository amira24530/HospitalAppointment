using HospitalAppointment.Models;

namespace HospitalAppointment.Extensions
{
    // This static class contains extension methods for the Doctor model.
    public static class DoctorExtensions
    {
        // This method calculates the total consultation charge.
        public static decimal CalculateTotalCharge(this Doctor doctor)
        {
            // Add ₹100 service charge to the doctor's consultation fee.
            return doctor.ConsultationFee + 100;
        }
    }
}