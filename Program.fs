open Suave
open Suave.Successful
open TodoDB

let serializeTodoList (tl:TodoList) =
    sprintf "{\"id\":%d,\"title\":\"%s\"}" tl.id tl.title

let serializeTodoSeq todos =
    Seq.map serializeTodoList todos
    |> String.concat ","
    |> sprintf "[%s]"

let getAllTodoLists s: WebPart =
    fun (x: HttpContext) ->
        async {
            let! lists = fetchAllTodoLists
            return! OK (s lists) x
        }

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig (getAllTodoLists serializeTodoSeq)
    0
