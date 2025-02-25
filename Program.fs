module Program

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

// Import your handlers
open Handlers

let webApp =
    choose [
        GET >=> route "/" >=> text "GiraffeTodo is running!"
        subRoute "/api" (
            choose [
                route "/todos" >=> TodoHandlers.getTodos
                route "/weather" >=> WeatherHandlers.getWeather
                route "/image" >=> ImageHandlers.getImage
            ]
        )
    ]

let configureApp (app: IApplicationBuilder) =
    app.UseGiraffe webApp

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    configureServices builder.Services
    let app = builder.Build()
    configureApp app
    app.Run()
    0