using dating_course_api.Src.Entities;
using Microsoft.AspNetCore.Identity;

namespace dating_course_api.Src.Validations
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var errors = new List<IdentityError>();

            if (string.IsNullOrWhiteSpace(user.UserName))
                errors.Add(
                    new IdentityError
                    {
                        Code = "InvalidUserName",
                        Description = "El nombre de usuario es requerido."
                    }
                );

            if (string.IsNullOrWhiteSpace(user.Email))
                errors.Add(
                    new IdentityError
                    {
                        Code = "InvalidEmail",
                        Description = "El correo electrónico es requerido."
                    }
                );

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var existingUser = await manager.FindByEmailAsync(user.Email);
                if (existingUser != null && !existingUser.Id.Equals(user.Id))
                    errors.Add(
                        new IdentityError
                        {
                            Code = "DuplicateEmail",
                            Description = "El correo electrónico ya está en uso."
                        }
                    );
            }

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed([.. errors]);
        }
    }
}
