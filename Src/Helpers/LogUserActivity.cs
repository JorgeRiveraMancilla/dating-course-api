using dating_course_api.Src.Data;
using dating_course_api.Src.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dating_course_api.Src.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var resultContext = await next();

            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
                return;

            var userId = resultContext.HttpContext.User.GetUserId();
            var dataContext =
                resultContext.HttpContext.RequestServices.GetRequiredService<DataContext>();
            var user = await dataContext.Users.FindAsync(userId);

            if (user is null)
                return;

            user.LastActive = DateTime.UtcNow;
            _ = await dataContext.SaveChangesAsync();
        }
    }
}
