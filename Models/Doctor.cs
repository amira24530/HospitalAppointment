using System;

namespace HospitalAppointment.Models
{
    // This class represents a doctor in the hospital appointment system.
    public class Doctor
    {
        // Stores the unique ID of the doctor.
        public int DoctorId { get; set; }

        // Stores the name of the doctor.
        public string DoctorName { get; set; } = string.Empty;

        // Stores the doctor's medical specialization.
        public string Specialization { get; set; } = string.Empty;

        // Stores the doctor's years of experience.
        public int Experience { get; set; }

        // Stores the doctor's consultation fee.
        public decimal ConsultationFee { get; set; }
    }
}