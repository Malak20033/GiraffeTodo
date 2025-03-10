module TodoRoutes

open Giraffe
open Handlers

let routes: HttpHandler =
    choose [
        GET >=> route "/api/todo" >=> getTodos
        POST >=> route "/api/todo" >=> addTodo
        routef "/api/todo/%i" deleteTodo >=> DELETE
    ]
