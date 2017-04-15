module TodoDB
    open PostgresDB
    open System.Data.Common

    type TodoList = {
        id: int
        title: string
    }

    let readerToTodoList (reader: DbDataReader) =
        { id = reader.GetInt32(0); title = reader.GetString(1) }

    let connection = connect "localhost" "panagiotis" "mypass" "todosdb"

    let fetchAllTodoLists =
        "select id, title from todo_list"
        |> select readerToTodoList connection
