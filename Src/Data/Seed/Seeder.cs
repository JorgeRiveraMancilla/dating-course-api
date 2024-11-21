using System.Text.Json;
using System.Text.Json.Serialization;
using dating_course_api.Src.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data.Seed
{
    public class Seeder
    {
        private static readonly JsonSerializerOptions _options =
            new()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new DateOnlyJsonConverter() }
            };

        public static async Task SeedUsers(
            UserManager<User> userManager,
            RoleManager<Role> roleManager
        )
        {
            if (await userManager.Users.AnyAsync())
                return;

            var userData = await File.ReadAllTextAsync("Src/Data/Seed/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<User>>(userData, _options);

            if (users == null)
                return;

            var roles = new List<Role>
            {
                new() { Name = "Member" },
                new() { Name = "Admin" },
                new() { Name = "Moderator" },
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.Photos.First().IsApproved = true;
                user.UserName = user.UserName!.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            // FIXME: Use credentials from configuration
            var admin = new User
            {
                UserName = "admin",
                Email = "",
                KnownAs = "Admin",
                Gender = "",
                City = "",
                Country = "",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
        }
    }

    public class UserSeedData
    {
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public required string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public required string Introduction { get; set; }
        public required string LookingFor { get; set; }
        public required string Interests { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required List<PhotoSeedData> Photos { get; set; }
    }

    public class PhotoSeedData
    {
        public required string Url { get; set; }
        public bool IsMain { get; set; }
    }

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            return DateOnly.ParseExact(
                reader.GetString() ?? throw new JsonException(),
                DateFormat,
                null
            );
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }
}
