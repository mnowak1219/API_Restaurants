namespace API_Restaurants.Middleware
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private readonly ILogger<RequestTimeMiddleware> _logger;

        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {            
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();

            var logMessage = $"Executing time for {context.Request.Method} at {context.Request.Path} is {_stopwatch.Elapsed}.";
            _logger.LogInformation(logMessage);

            _stopwatch.Reset();
        }
    }
}
