using Library.Core;
using Library.Core.RepositoryContract;
using Library.Core.ServiceContract;
using Library.Repository;
using Library.Service;
using LibraTrack.API.Errors;
using LibraTrack.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LibraTrack.API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IBookService), typeof(BookService));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped(typeof(IBorrowingService),typeof(BorrowingService));
            services.AddScoped(typeof(IFineService),typeof(FineService));
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(IRatingsService), typeof(RatingService));
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IBookRepository, BookRepository>();

            services.AddAutoMapper(typeof(MappingProfiles));

            services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });
            return services;
        }
    }
}
