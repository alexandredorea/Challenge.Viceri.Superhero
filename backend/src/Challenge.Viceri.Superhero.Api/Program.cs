using Challenge.Viceri.Superhero.Api;

await WebApplication.CreateBuilder(args)
    .RegisterServices().Build()
    .UseServices().RunAsync();