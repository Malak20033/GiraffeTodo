open Fable.React
open Fable.React.Props
open Fable.Fetch
open System
open Models.Todo

let mutable todos: TodoItem list = []

// Fetch todos from the backend API
let fetchTodos() =
    async {
        let! response = Fetch.fetch "/api/todo" [] |> Async.AwaitPromise
        let! result = response.text() |> Async.AwaitPromise
        todos <- Newtonsoft.Json.JsonConvert.DeserializeObject<TodoItem list>(result)
        ReactDom.render(todoList(), Browser.Dom.document.getElementById("todo-list-ul"))
    }

// Add a new todo
let addTodo (task: string) =
    async {
        let newTodo = createTodoItem (DateTime.Now.Ticks |> int) task
        let! _ = Fetch.fetch("/api/todo", Fetch.requestBodyJson newTodo) |> Async.AwaitPromise
        todos <- todos @ [newTodo]
        fetchTodos()  // Re-fetch and update the UI
    }

// Complete a todo
let completeTodo (id: int) =
    async {
        let! _ = Fetch.fetch("/api/todo/complete", Fetch.requestBodyJson id) |> Async.AwaitPromise
        todos <- todos |> List.map (fun t -> if t.Id = id then completeTodoItem t else t)
        fetchTodos()  // Re-fetch and update the UI
    }

// Delete a todo
let deleteTodo (id: int) =
    async {
        let! _ = Fetch.fetch(sprintf "/api/todo/%i" id, Fetch.requestMethod "DELETE") |> Async.AwaitPromise
        todos <- todos |> List.filter (fun t -> t.Id <> id)
        fetchTodos()  // Re-fetch and update the UI
    }

// Render the todo list
let todoList () =
    div [] [
        h2 [] [str "To-Do List"]
        ul [] (todos |> List.map (fun todo ->
            li [] [
                str todo.Task
                if not todo.IsCompleted then
                    button [OnClick (fun _ -> completeTodo todo.Id)] [str "Complete"]
                else
                    str "Completed"
                button [OnClick (fun _ -> deleteTodo todo.Id)] [str "Delete"]
            ]
        ))
    ]

// Main app
let app () =
    React.useEffectOnce (fun () -> fetchTodos())  // Fetch todos when the component mounts
    div [] [
        todoList ()
    ]

ReactDOM.render(app(), Browser.Dom.document.getElementById("root"))