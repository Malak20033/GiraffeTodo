module EmailHandler

open System
open System.Net
open System.Net.Mail
open System.Threading
open System.Net.Http
open Newtonsoft.Json
open DotNetEnv

Env.Load() |> ignore 
let emailPassword = Env.GetString "EMAIL_PASSWORD"

// Helper function to fetch data from an API
let fetchData<'T> (url: string) =
    async {
        printfn "Fetching data from: %s" url
        use client = new HttpClient()
        try
            let! response = client.GetStringAsync(url) |> Async.AwaitTask
            printfn "Data fetched successfully from: %s" url
            return JsonConvert.DeserializeObject<'T>(response)
        with
        | ex ->
            printfn "Error fetching data from %s: %s" url ex.Message
            return Unchecked.defaultof<'T>
    }

// Define the Todo type to match the format of the data
type Todo = {
    Id: int
    Title: string
    Completed: bool
}

// Function to send an email with todos, weather, and image
let sendEmail todos weather imageUrl =
    try
        // Define SMTP client
        use smtpClient = new SmtpClient("smtp.gmail.com")
        smtpClient.Port <- 587
        smtpClient.Credentials <- NetworkCredential("malakshakhtour@gmail.com", emailPassword) // Use a secure method for storing passwords
        smtpClient.EnableSsl <- true

        // Build the email content
        let todoList = 
            todos 
            |> List.map (fun todo -> 
                sprintf "- %s (Completed: %b)" todo.Title todo.Completed)
            |> String.concat "\n"

        let emailBody = 
            "<html>" +
            "<body>" +
            "<h1>Hiya</h1>" +
            "<h2>To-Do List:</h2>" +
            $"<pre>{todoList}</pre>" + 
            "<h2>Weather:</h2>" +
            $"<p>{weather}</p>" +
            "<h2>Image of the Day:</h2>" +
            "<img src=\"" + imageUrl + "\" alt=\"Image of the day\" style=\"max-width: 100%; height: auto;\" />" +
            "<p>Good luck</p>" +
            "</body>" +
            "</html>"

        // Define the email
        let mail = new MailMessage()
        mail.From <- MailAddress("malakshakhtour@gmail.com")
        mail.To.Add("malakshakhtour@gmail.com")
        mail.Subject <- "Daily Updates"
        mail.Body <- emailBody
        mail.IsBodyHtml <- true // Enable HTML in email content

        // Send the email
        smtpClient.Send(mail)
        printfn "Email sent successfully."
    with
    | ex -> printfn "Failed to send email: %s" ex.Message

// Function to schedule daily emails
let scheduleDailyEmail () =
    printfn "Scheduling daily email..."
    let now = DateTime.Now
    let targetTime = DateTime(now.Year, now.Month, now.Day, 6, 00, 0) 
    let initialDelay =
        if now > targetTime then
            targetTime.AddDays(1.0) - now // Schedule for tomorrow if time has passed
        else
            targetTime - now // Schedule for today

    // Convert delay to milliseconds
    let delayInMilliseconds = int initialDelay.TotalMilliseconds

    // Set up a timer to fetch data and send the email
    let timer = new Timer((fun _ -> 
        async {
            printfn "Fetching data for email..."
            // Fetch todos, weather, and image data
            let! todos = fetchData<Todo list>("http://localhost:5000/api/todo")
            let! weatherData = fetchData<{| currentTemperature: float |}>("http://localhost:5000/api/weather")
            let weather = $"Current temperature: {weatherData.currentTemperature}°F"
            let! imageData = fetchData<{| imageUrl: string |}>("http://localhost:5000/api/image")

            // Extract the image URL
            let imageUrl = imageData.imageUrl

            // Send the email
            sendEmail todos weather imageUrl
        } |> Async.Start
    ), null, delayInMilliseconds, 24 * 60 * 60 * 1000) // Repeat every 24 hours

    printfn "Email scheduled for 6 AM daily."
    timer
