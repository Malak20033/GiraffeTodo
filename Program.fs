open System
open Handlers
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Handlers
open TodoRoutes


// ---------------------------------
// WebApp module (Contains HttpHandler)
// ---------------------------------
module WebApp =

    let webApp : HttpHandler = 
        choose [
            route "/" >=> htmlFile "WebRoot/index.html"
            TodoRoutes.routes
            route "/api/weather" >=> WeatherHandler.weatherHandler
            route "/api/image" >=> ImageHandler.getImageUrlHandler
            setStatusCode 404 >=> text "Not Found" 
    ]
// ---------------------------------
// Error handler
// ---------------------------------
    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

    let configureCors (builder : CorsPolicyBuilder) =
        builder
            .WithOrigins(
                "http://localhost:5000",
            "https://localhost:5001")
            .AllowAnyMethod()
            .AllowAnyHeader()
        |> ignore

    let configureApp (app : IApplicationBuilder) =
        let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
        (match env.IsDevelopment() with
        | true  ->
        app.UseDeveloperExceptionPage()
            .UseCors(configureCors)
        | false ->
            app.UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
            .UseCors(configureCors)
            .UseStaticFiles()
            .UseGiraffe(webApp)

    let configureServices (services : IServiceCollection) =
        services.AddCors()     |> ignore
        services.AddGiraffe()  |> ignore

    let configureLogging (builder : ILoggingBuilder) =
        builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")

    EmailHandler.scheduleDailyEmail() |> ignore

    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> WebApp.configureApp)
                    .ConfigureServices(WebApp.configureServices)
                    .ConfigureLogging(WebApp.configureLogging)
                    
                    |> ignore)
        .Build()
        .Run()
    0