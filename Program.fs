namespace TodoController

open Giraffe
open Microsoft.AspNetCore.Http
open System.Collections.Generic
open Models.Todo

module TodoModule =

    // Use the TodoItem type from Models.Todo
    let mutable todos: TodoItem list = [
        { Id = 1; Task = "Learn F#"; IsCompleted = false }
        { Id = 2; Task = "Build a web app"; IsCompleted = false }
    ]

    let getAllTodos : HttpHandler =
        fun next ctx ->
            let jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(todos)
            Successful.OK jsonResponse next ctx

    let addTodo : HttpHandler =
        fun next ctx ->
            task {
                let! newTodo = ctx.BindJsonAsync<TodoItem>()
                todos <- todos @ [newTodo]
                return! Successful.OK "Todo added" next ctx
            }

    let completeTodo : HttpHandler =
        fun next ctx ->
            task {
                let! id = ctx.BindJsonAsync<int>()
                todos <- todos |> List.map (fun t -> if t.Id = id then { t with IsCompleted = true } else t)
                return! Successful.OK "Todo marked as completed" next ctx
            }

    let deleteTodo : HttpHandler =
        fun next ctx ->
            task {
                let! id = ctx.BindJsonAsync<int>()
                todos <- todos |> List.filter (fun t -> t.Id <> id)
                return! Successful.OK "Todo deleted" next ctx
            }