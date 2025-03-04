namespace Handlers

open Giraffe
open FSharp.Control.TaskBuilder
open Microsoft.AspNetCore.Http
open System.IO

module Handlers =
    // Simulated in-memory todo list
    let todos = ref ["Buy groceries"; "Finish project"; "Read book"]
    
    let handler (next: HttpFunc) (ctx: HttpContext) =
        let dataObj = todos.Value
        json dataObj next ctx
