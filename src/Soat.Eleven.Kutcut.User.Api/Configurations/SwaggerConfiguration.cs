using Microsoft.OpenApi;
using System.Reflection;

namespace Soat.Eleven.Kutcut.Users.Api.Configurations;

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SOAT - KutCut - User Microservice",
                Version = "v1",
                Description = @"
Projeto acadêmico desenvolvido para a disciplina de Arquitetura de Software (FIAP - Pós-graduação)

API para gerenciamento de usuários do sistema KutCut

**Importante:** Sempre envie o token JWT no header `Authorization: Bearer {token}` para acessar os endpoints protegidos.",
                Contact = new OpenApiContact
                {
                    Name = "SOAT Eleven Team",
                    Email = "team@soateleven.com"
                }
            });

            // Configuração de autenticação JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: Bearer {seu token}"
            });



            //c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme {
            //            BearerFormat = "JWT",
            //            Description = "Insira o token JWT no formato: Bearer {seu token}",
            //            Name = "Authorization",
            //            Type = SecuritySchemeType.Http,
            //        },
            //        Array.Empty<string>()
            //    }
            //});

            // Incluir comentários XML se existirem
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Configurar tags para organizar os endpoints
            c.TagActionsBy(api =>
            {
                var controllerName = api.ActionDescriptor.RouteValues["controller"];
                return [controllerName ?? "Default"];
            });
        });
    }

    public static void UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Microservice v1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "KutCut User API";

                // Configurações de UI
                c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });
        }
    }
}
