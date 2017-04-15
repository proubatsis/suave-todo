DROP TABLE IF EXISTS todo_item;
DROP TABLE IF EXISTS todo_list;

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

INSERT INTO todo_list (title) VALUES ('backlog'), ('in progress');
INSERT INTO todo_item (todo_list_id, title, completed) VALUES
	(1, 'do stuff', false),
	(2, 'bark at dog', true),
	(1, 'do not do stuff', true);

