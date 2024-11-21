namespace dating_course_api.Src.Extensions
{
    public static class DateOnlyExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - dob.Year;

            if (dob > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
