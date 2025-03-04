namespace Handlers

open Giraffe
open FSharp.Control.Tasks.V2
open Microsoft.AspNetCore.Http
open System.IO

// Simulated in-memory todo list
let todos = ref ["Buy groceries"; "Finish project"; "Read book"]

module TodoHandlers =
    let getTodos: HttpHandler =
        fun next ctx ->
            json !todos next ctx

module WeatherHandlers =
    let getWeather: HttpHandler =
        fun next ctx ->
            // Your weather handler code here
            text "Weather data" next ctx

module ImageHandlers =
    let getImage: HttpHandler =
        fun next ctx ->
            // Your image handler code here
            text "Image data" next ctx

        