open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open TodoDB
open Newtonsoft.Json
open System.Net

let serialize o =
    JsonConvert.SerializeObject(o)

let getAllTodoLists ctx =
    async {
        let! lists = fetchAllTodoLists()
        return! OK (serialize lists) ctx
    }

let jsonMime = setMimeType "application/json; charset=utf-8"

[<EntryPoint>]
let main argv =
    let api =
        [
            GET >=> path "/todos" >=> getAllTodoLists >=> jsonMime
        ] |> choose

    let rec start (port: uint16) =
        let config = 
            { defaultConfig with 
                bindings=[ HttpBinding.create HTTP (IPAddress.Parse("0.0.0.0")) port ]
            } in
            try
                startWebServer config api
                0
            with
                | :? Sockets.SocketException -> start (port + 1us)
                | _ -> -1
        
    start 8080us
