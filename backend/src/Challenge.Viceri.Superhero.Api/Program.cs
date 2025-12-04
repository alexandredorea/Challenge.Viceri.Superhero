using Challenge.Viceri.Superhero.Api;

await WebApplication.CreateBuilder(args)
    .RegisterServices().Build()
    .UseServices().RunAsync();

/// <summary>
///
/// </summary>
public partial class Program
{
    /// <summary>
    ///
    /// </summary>
    protected Program()
    {
    }
}