DROP TABLE IF EXISTS todo_list CASCADE;

CREATE TABLE todo_list (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255)
);

CREATE TABLE todo_item (
    id SERIAL PRIMARY KEY,
    todo_list_id INTEGER REFERENCES todo_list(id),
    title VARCHAR(255),
    completed BOOLEAN
);
