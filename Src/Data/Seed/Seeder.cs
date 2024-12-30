using Bogus;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data.Seed
{
    public static class Seeder
    {
        public static async Task SeedUsers(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration config,
            IWebHostEnvironment env,
            IPhotoService photoService
        )
        {
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager, config);

            if (env.IsDevelopment())
            {
                await SeedMemberUsers(userManager, env, photoService);
            }
        }

        private static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            var roleNames = new[] { "Member", "Admin", "Moderator" };

            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var role = new Role { Name = roleName };
                var roleResult = await roleManager.CreateAsync(role);

                if (!roleResult.Succeeded)
                    throw new Exception(
                        $"Error creating role '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}"
                    );
            }
        }

        private static async Task SeedAdminUser(
            UserManager<User> userManager,
            IConfiguration config
        )
        {
            if (await userManager.Users.AnyAsync(u => u.UserName == config["AdminUser:UserName"]))
                return;

            var birthDateStr = config["AdminUser:BirthDate"]!;
            var birthDate = DateOnly.Parse(birthDateStr);
            var password = config["AdminUser:Password"]!;

            var admin = new User
            {
                UserName = config["AdminUser:UserName"]!,
                Email = config["AdminUser:Email"]!,
                KnownAs = config["AdminUser:KnownAs"]!,
                Gender = config["AdminUser:Gender"]!,
                City = config["AdminUser:City"]!,
                Country = config["AdminUser:Country"]!,
                BirthDate = birthDate
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new Exception(
                    $"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );

            var createdUser =
                await userManager.FindByEmailAsync(admin.Email)
                ?? throw new Exception("Failed to retrieve the newly created admin user.");

            var roleAssignmentResult = await userManager.AddToRolesAsync(
                createdUser,
                ["Admin", "Moderator"]
            );

            if (!roleAssignmentResult.Succeeded)
                throw new Exception(
                    $"Error assigning roles to admin user: {string.Join(", ", roleAssignmentResult.Errors.Select(e => e.Description))}"
                );
        }

        private static async Task SeedMemberUsers(
            UserManager<User> userManager,
            IWebHostEnvironment env,
            IPhotoService photoService
        )
        {
            if (1 < await userManager.Users.CountAsync())
                return;

            var defaultPassword = "Passw0rd";
            var faker = new Faker();

            for (var i = 0; i < 100; i++)
            {
                var gender = faker.PickRandom("male", "female");
                var firstName = faker.Name.FirstName();
                var userName = faker.Internet.UserName(firstName).ToLower();

                if (await userManager.Users.AnyAsync(u => u.UserName == userName))
                    continue;

                var user = new User
                {
                    UserName = userName,
                    Email = faker.Internet.Email(firstName),
                    KnownAs = firstName,
                    Gender = gender,
                    City = faker.Address.City(),
                    Country = faker.Address.Country(),
                    BirthDate = DateOnly.FromDateTime(
                        faker.Date.Past(30, DateTime.Now.AddYears(-18))
                    ),
                    Introduction = faker.Lorem.Paragraph(),
                    LookingFor = faker.Lorem.Sentence(),
                    Interests = string.Join(", ", faker.Lorem.Words(5))
                };

                var result = await userManager.CreateAsync(user, defaultPassword);
                if (!result.Succeeded)
                    throw new Exception(
                        $"Error creating member user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                    );

                var createdUser =
                    await userManager.FindByEmailAsync(user.Email)
                    ?? throw new Exception(
                        $"Failed to retrieve the newly created user: {user.Email}"
                    );

                await userManager.AddToRoleAsync(createdUser, "Member");

                var imagePath = Path.Combine(env.WebRootPath, "images", "seed", "user.png");

                await using var stream = File.OpenRead(imagePath);
                var file = new FormFile(
                    baseStream: stream,
                    baseStreamOffset: 0,
                    length: stream.Length,
                    name: "image",
                    fileName: "user.png"
                );

                var uploadResult = await photoService.CreatePhotoAsync(file);
                if (uploadResult.Error != null)
                    throw new Exception($"Error uploading photo: {uploadResult.Error.Message}");

                var photo = new Photo
                {
                    Url = uploadResult.SecureUrl.AbsoluteUri,
                    PublicId = uploadResult.PublicId,
                    IsMain = true,
                    IsApproved = true,
                    User = createdUser
                };

                createdUser.Photos.Add(photo);
                await userManager.UpdateAsync(createdUser);
            }
        }
    }
}
