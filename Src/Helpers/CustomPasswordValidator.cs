using dating_course_api.Src.Entities;
using Microsoft.AspNetCore.Identity;

namespace dating_course_api.Src.Helpers
{
    public class CustomPasswordValidator : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(
            UserManager<User> manager,
            User user,
            string? password
        )
        {
            if (
                string.IsNullOrEmpty(password)
                || password.Length < 8
                || 20 < password.Length
                || !password.All(char.IsLetterOrDigit)
            )
            {
                return Task.FromResult(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "PasswordValidationFailed",
                            Description =
                                "Password must be between 8 and 20 characters and contain letters and numbers"
                        }
                    )
                );
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
