namespace RestaurantMS_test.Middelware
{
    public class HoursMiddleware
    {
        public  RequestDelegate _next;

        public HoursMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            int hour = DateTime.Now.Hour;

            if (hour < 9 || hour >= 22)//open from 9am to 10pm
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Sorry, the restaurant is closed.");
                return;
            }

            await _next(context);
        }
    }
}
