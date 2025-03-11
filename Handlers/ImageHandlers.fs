module ImageHandler

open FSharp.Data
open DotNetEnv
open Giraffe

Env.Load() |> ignore // Ensure .env file is loaded
let apiKey = Env.GetString "PEXELS_API_KEY"  // Use `Env.get` instead of `Env.GetString`

let pexelsPhotoId = "208984"

// Function to get the image URL from Pexels API
let getPexelsImageUrl () =
    let url = $"https://api.pexels.com/v1/photos/{pexelsPhotoId}"
    let headers = [ "Authorization", apiKey ]
    
    try
        let response = Http.RequestString(url, headers = headers)
        let json = JsonValue.Parse(response)
        match json.TryGetProperty("src") with
        | Some src -> src.GetProperty("original").AsString()
        | None -> failwith "No image found"
    with
    | ex -> sprintf "Error: %s" ex.Message

// Giraffe handler to serve the image URL as JSON
let getImageUrlHandler : HttpHandler =
    fun next ctx ->
        task {
            try
                let imageUrl = getPexelsImageUrl()
                return! json {| imageUrl = imageUrl |} next ctx
            with
            | ex -> 
                return! json {| error = sprintf "Failed to fetch image: %s" ex.Message |} next ctx
        }