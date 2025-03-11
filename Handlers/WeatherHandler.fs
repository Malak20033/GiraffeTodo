module WeatherHandler

open System.Net.Http
open Newtonsoft.Json
open Giraffe

type HourlyWeather =
    { Time: string[]
      [<JsonProperty("temperature_2m")>]
      Temperature2m: float[] }

type WeatherResponse =
    { Hourly: HourlyWeather }



let getWeatherData (latitude: float) (longitude: float) : Async<WeatherResponse option> =
    async {
    // Build the API URL
    let url = sprintf "https://api.open-meteo.com/v1/forecast?latitude=%f&longitude=%f&hourly=temperature_2m&temperature_unit=fahrenheit" latitude longitude

    // Create an HTTP client
    use client = new HttpClient()


    // Make the request asynchronously
    let! response = client.GetStringAsync(url) |> Async.AwaitTask

    

    // Try to parse the response into a WeatherResponse
    try
        let weather = JsonConvert.DeserializeObject<WeatherResponse>(response)
        return Some weather
    with
    | _ -> return None
}
// Giraffe route that returns weather data
let weatherHandler : HttpHandler =
    printfn "Weather handler called"
    fun next ctx ->
        task {
            // Get the weather data for a specific location
            let! weatherData = getWeatherData 37.6922 -97.3375
           
            match weatherData with
            | Some data ->
                // Extract the current temperature (assumes the first value corresponds to current time)
                match data.Hourly.Temperature2m |> Array.tryHead with
                | Some currentTemp ->
                    // Return only the current temperature as JSON
                    return! json {| currentTemperature = currentTemp |} next ctx
                | None ->
                    // Handle the case where the temperature array is empty
                    return! json {| error = "No temperature data available" |} next ctx
            | None ->
                // If there was an issue, return an error message
                return! json {| error = "Unable to fetch weather data" |} next ctx
        }