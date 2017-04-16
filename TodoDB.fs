module TodoDB
    open PostgresDB
    open System.Data.Common

    type TodoList = {
        id: int
        title: string
    }

    type TodoItem = {
        id: int
        todoListId: int
        title: string
        completed: bool
    }

    type Todo = {
        id: int
        title: string
        items: TodoItem seq
    }

    let readerToTodoList (reader: DbDataReader) =
        { id = reader.GetInt32(0); title = reader.GetString(1) }

    let readerToTodoItem todoListId (reader: DbDataReader) =
        { id = reader.GetInt32(0); todoListId = todoListId; title = reader.GetString(1); completed = reader.GetBoolean(2) }

    let connection = connect "localhost" "panagiotis" "mypass" "todosdb"

    let fetchAllTodoLists() =
        select connection readerToTodoList "select id, title from todo_list" []

    let fetchTodo (id: int) =
        async {
            let! results =
                [ { name = "id"; value=id } ]
                |> select connection readerToTodoList "select id, title from todo_list where id=@id"
            
            return!
                match (Seq.first results) with
                | Some(tl) ->
                    async {
                        let! items =
                            [ { name="todoListId"; value=id } ]
                            |> select connection (readerToTodoItem id) "select id, title, completed from todo_item where todo_list_id=@todoListId"
                        return Some { id = id; title = tl.title; items = items }
                    }
                | _ -> async { return None }
        }
