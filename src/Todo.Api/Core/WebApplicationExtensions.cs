using MediatR;

namespace Todo.Api
{
    public static class WebApplicationExtensions
    {
        public static IEndpointConventionBuilder MapGet<TRequest>(
            this WebApplication app,
            string pattern
            )
            where TRequest : IBaseRequest, new()
        {
            return app.MapGet(pattern, RequestHandler<TRequest>);
        }

        private static async Task RequestHandler<TRequest>(HttpContext httpContext)
            where TRequest : IBaseRequest, new()
        {
            var _mediator = httpContext.RequestServices.GetRequiredService<IMediator>();
            var request = await httpContext.ModelBindAsync<TRequest>();

            await _mediator.Send(request);
        }

        private static async Task<TRequest> ModelBindAsync<TRequest>(this HttpContext ctx)
            where TRequest : IBaseRequest, new()
        {
            var requestType = typeof(TRequest);
            var interfaces = requestType.GetInterfaces();

            TRequest result = interfaces.Any(x => x.Equals(typeof(IFromJsonBody)))
                ? (TRequest)await ctx.Request.ReadFromJsonAsync(requestType)
                : new TRequest();

           if(result is IFromRoute fromRoute)
                fromRoute.BindFromRoute(ctx.Request.RouteValues);

            return result;
        }
    }
}