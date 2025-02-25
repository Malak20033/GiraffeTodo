module WeatherHandlers

open System.Net.Http
open Giraffe
open Newtonsoft.Json.Linq

let httpClient = new HttpClient()

let getWeather: HttpHandler =
    fun next ctx ->
        task {
            let! response = httpClient.GetStringAsync("https://api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.0060&current_weather=true")
            let json = JObject.Parse(response)
            return! json json next ctx
        }