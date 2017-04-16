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
        items: TodoItem list
    }

    let readerToTodoList (reader: DbDataReader) =
        { id = reader.GetInt32(0); title = reader.GetString(1) }

    let readerToTodoItem (reader: DbDataReader) =
        { id = reader.GetInt32(0); todoListId = reader.GetInt32(1); title = reader.GetString(2); completed = reader.GetBoolean(3) }

    let readerToTodo (reader: DbDataReader) =
        {
            id = reader.GetInt32(0)
            title = reader.GetString(1)
            items =
                try
                    [
                        {
                            id = reader.GetInt32(2)
                            todoListId = reader.GetInt32(0)
                            title = reader.GetString(3)
                            completed = reader.GetBoolean(4)
                        }
                    ]
                with
                | _ -> []
        }

    let connection = connect "localhost" "panagiotis" "mypass" "todosdb"

    let fetchAllTodoLists() =
        select connection readerToTodoList "select id, title from todo_list" []

    let fetchTodo (id: int) =
        async {
            let sql =
                "select a.id as id, a.title as list_title, b.id as item_id, b.title as item_title, b.completed
                from todo_list a
                left join todo_item b
                on a.id=b.todo_list_id
                where a.id=@id"

            let! results = select connection readerToTodo sql [ { name = "id"; value = id } ]
            return
                if Seq.isEmpty results then
                    None
                else
                    Seq.reduce (fun a t -> { a with items = (List.head t.items)::a.items }) results
                    |> Some
        }

    let fetchItem ((todoListId: int), (itemId: int)) =
        async {
            let sql =
                "select id, todo_list_id, title, completed
                from todo_item
                where id=@id and todo_list_id=@todoListId"

            let! results =
                [
                    { name = "todoListId"; value = todoListId }
                    { name = "id"; value = itemId }
                ] |> select connection readerToTodoItem sql

            return Seq.first results
        }
