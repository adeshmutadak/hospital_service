using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal static class PersonDisplay
    {
        public static string? GenderName(byte? gender) => gender switch
        {
            1 => "Female",
            2 => "Male",
            3 => "Other",
            _ => null
        };

        public static int? Age(DateOnly? dob)
        {
            if (!dob.HasValue) return null;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dob.Value.Year;
            if (dob.Value > today.AddYears(-age)) age--;
            return age;
        }

        public static string? PatientPhotoUrl(long patientId, string? photoPath) =>
            photoPath == null ? null : $"/v1/api/Patient/{patientId}/photo";
    }
}
