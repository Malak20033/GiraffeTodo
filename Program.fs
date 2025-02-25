module Program

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Handlers
open Handlers.TodoHandlers
open Handlers.WeatherHandlers
open Handlers.ImageHandlers

let webApp =
    choose [
        GET >=> route "/" >=> text "GiraffeTodo is running!"
        subRoute "/api" (
            choose [
                route "/todos" >=> getTodos
                route "/weather" >=> getWeather
                route "/image" >=> getImage
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