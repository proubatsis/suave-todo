open Suave
open Suave.Successful
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
    startWebServer defaultConfig (getAllTodoLists serializeTodoSeq)
    0
