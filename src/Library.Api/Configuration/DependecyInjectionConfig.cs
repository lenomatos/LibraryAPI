using Library.Business.Interfaces;
using Library.Data.Context;
using Library.Data.Repositorys;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Api.Configuration
{
    public static class DependecyInjectionConfig
    {
        public static IServiceCollection AddDenpendencyConfig(this IServiceCollection services)
        {
            services.AddScoped<DataContext>();

            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookGenreRepository, BookGenreRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IReserveRepository, ReserveRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
