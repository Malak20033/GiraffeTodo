module Handlers

open Giraffe
open FSharp.Control.Tasks.V2
open Microsoft.AspNetCore.Http
open System.IO

// Simulated in-memory todo list
let todos = ref ["Buy groceries"; "Finish project"; "Read book"]

let getTodos: HttpHandler =
    fun next ctx ->
        json !todos next ctx