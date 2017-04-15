module TodoDB
    open Npgsql
    open System.Data.Common

    type TodoList = {
        id: int
        title: string
    }

    let readerToTodoList (reader: DbDataReader) =
        { id = reader.GetInt32(0); title = reader.GetString(1) }

    let connection = new NpgsqlConnection("Host=localhost;Username=panagiotis;Password=mypass;Database=todosdb")
    connection.Open()

    let select readerToRecord q =
        async {
            let mutable cmd = new NpgsqlCommand()
            cmd.Connection <- connection
            cmd.CommandText <- q
            
            let! reader = Async.AwaitTask (cmd.ExecuteReaderAsync())
            return seq {
               while reader.Read() do yield readerToRecord reader
               reader.Dispose()
               cmd.Dispose()
            }
        }

    let fetchAllTodoLists =
        "select id, title from todo_list"
        |> select readerToTodoList
