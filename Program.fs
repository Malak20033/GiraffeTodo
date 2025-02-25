open System
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open System.Net.Http
open System.Threading.Tasks
open Microsoft.AspNetCore.Http

// Define Todo type
type Todo = { Id: int; Task: string; Done: bool }
let mutable todos = [{ Id = 1; Task = "Finish F# project"; Done = false }]

// Handlers
let getTodosHandler = fun (next: HttpFunc) (ctx: HttpContext) ->
    json todos next ctx

let addTodoHandler = fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
        let! newTodo = ctx.BindJsonAsync<Todo>()
        todos <- newTodo :: todos
        return! json todos next ctx
    }

// Fetch weather data
let getWeatherData () : Task<string> =
    task {
        use client = new HttpClient()
        let! response = client.GetStringAsync("https://api.open-meteo.com/v1/forecast?...&current_weather=true")
        return response
    }

// Fetch image
let getImageUrl () : Task<string> =
    task {
        use client = new HttpClient()
        let! response = client.GetStringAsync("https://api.unsplash.com/photos/random?client_id=YOUR_API_KEY")
        return response
    }

// Email notification (simplified placeholder)
let sendDailyEmail () =
    printfn "Sending email with todos, weather, and image..."

// Web app setup
let webApp =
    choose [
        GET >=> route "/todos" >=> getTodosHandler
        POST >=> route "/todos" >=> addTodoHandler
    ]

let configureApp (app: IApplicationBuilder) =
    app.UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    let builder = WebApplication.CreateBuilder()
    configureServices builder.Services
    let app = builder.Build()
    configureApp app
    app.Run()
    0

