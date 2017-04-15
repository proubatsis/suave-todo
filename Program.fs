open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open TodoDB
open Newtonsoft.Json

let serializeTodoSeq (todos: TodoList seq) =
    JsonConvert.SerializeObject(todos)

let getAllTodoLists s: WebPart =
    fun (x: HttpContext) ->
        async {
            let! lists = fetchAllTodoLists()
            return! OK (s lists) x
        }

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig ((getAllTodoLists serializeTodoSeq) >=> setMimeType "application/json; charset=utf-8")
    0
