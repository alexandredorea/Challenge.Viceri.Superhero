using Challenge.Viceri.Superhero.Api.Middlewares;

namespace Challenge.Viceri.Superhero.Api;

/// <summary>
///
/// </summary>
public static class UseRegister
{
    /// <summary>
    /// Configura a pipeline de requisições HTTP.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseServices(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DefaultModelsExpandDepth(-1);
            });
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}