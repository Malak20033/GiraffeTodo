module Handlers

open System
open Giraffe
open Microsoft.AspNetCore.Http
open System.Threading

type Todo = {
    Id: int
    Title: string
    Completed: bool
}

let mutable todos: Todo list = [
    { Id = 1; Title = "Learn F#"; Completed = false }
    { Id = 2; Title = "Build a Todo app"; Completed = false }
    { Id = 3; Title = "Check emails"; Completed = true }
]
let idCounter = ref 1

let getTodos =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        json todos next ctx

let addTodo =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! newTodo = ctx.BindJsonAsync<Todo>()
            let todo = { newTodo with Id = Interlocked.Increment(idCounter) }
            todos <- todos @ [todo]
            return! json todo next ctx
        }

let deleteTodo (id: int) =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        match List.tryFind (fun t -> t.Id = id) todos with
        | Some _ ->
            todos <- List.filter (fun t -> t.Id <> id) todos
            text "Deleted successfully" next ctx
        | None -> RequestErrors.NOT_FOUND "Todo not found" next ctx