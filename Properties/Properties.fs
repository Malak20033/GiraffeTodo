namespace GiraffeTodo

open Giraffe
open Microsoft.AspNetCore.Http
open System.Collections.Generic
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

type TodoItem = {
    Id: int
    Task: string
    IsCompleted: bool
}

type TodoState = {
    Todos: TodoItem list
}

module TodoModule =

    let initialState = {
        Todos = [
            { Id = 1; Task = "Learn F#"; IsCompleted = false }
            { Id = 2; Task = "Build a Todo App"; IsCompleted = true }
        ]
    }

    let mutable state = initialState

    // Handler to serve the Todo list as JSON
    let getTodosHandler (next: HttpFunc) (ctx: HttpContext) =
        json state.Todos next ctx

    // Handler to add a new Todo item
    let addTodoHandler (next: HttpFunc) (ctx: HttpContext) =
        task {
            let! newTodo = ctx.BindJsonAsync<TodoItem>()
            state <- { state with Todos = newTodo :: state.Todos }
            return! json state.Todos next ctx
        }

    // Handler to update a Todo item's completion status
    let updateTodoHandler (id: int) (next: HttpFunc) (ctx: HttpContext) =
        task {
            let todoToUpdate = state.Todos |> List.tryFind (fun t -> t.Id = id)
            match todoToUpdate with
            | Some(todo) ->
                let updatedTodo = { todo with IsCompleted = not todo.IsCompleted }
                state <- { state with Todos = state.Todos |> List.map (fun t -> if t.Id = id then updatedTodo else t) }
                return! json updatedTodo next ctx
            | None -> return! setStatusCode 404 next ctx
        }
module App =

   
// Define your app's routing= 
    let webApp =
        choose [
        GET >=>
            choose [
                route "/api/todos" >=> TodoModule.getTodosHandler
            ]
        POST >=>
            choose [
                route "/api/todos" >=> TodoModule.addTodoHandler
            ]
        PATCH >=>
            choose [
        ]
    ]
    let configureApp (app: IApplicationBuilder) =
        app.UseGiraffe webApp

    let configureServices (services: IServiceCollection) : unit =
        services.AddGiraffe() |> ignore