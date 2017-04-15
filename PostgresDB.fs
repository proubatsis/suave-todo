module PostgresDB
    open Npgsql
    open System.Data.Common

    let connect host username password database =
        let conn = new NpgsqlConnection(sprintf "Host=%s;Username=%s;Password=%s;Database=%s" host username password database)
        conn.Open()
        conn

    let select readerToRecord connection q =
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