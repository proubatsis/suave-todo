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
        let rec readSequentially (reader: DbDataReader) (command: NpgsqlCommand) =
            async {
                let! hasNext = reader.ReadAsync()
                if hasNext then
                    let tl = readerToRecord reader
                    let! nextseq = readSequentially reader command
                    return seq {
                        yield tl
                        yield! nextseq
                    }
                else
                    reader.Dispose()
                    command.Dispose()
                    return Seq.empty
            }

        async {
            let mutable cmd = new NpgsqlCommand()
            cmd.Connection <- connection
            cmd.CommandText <- q
            
            let! reader = Async.AwaitTask (cmd.ExecuteReaderAsync())
            return! readSequentially reader cmd
        }

    let fetchAllTodoLists =
        "select id, title from todo_list"
        |> select readerToTodoList
