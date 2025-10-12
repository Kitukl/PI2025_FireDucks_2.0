CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    surname VARCHAR(100),
    email VARCHAR(150) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    userPhoto TEXT,
    isNotified BOOLEAN DEFAULT FALSE,
    daysForNotification INT
);

-- Таблиця Lectures
CREATE TABLE Lectures (
    lecture_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    surname VARCHAR(100)
);

-- Таблиця Subjects
CREATE TABLE Subjects (
    subject_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- Таблиця Lessons_Slots
CREATE TABLE Lessons_Slots (
    slot_id SERIAL PRIMARY KEY,
    startLesson TIME NOT NULL,
    endLesson TIME NOT NULL
);

-- Таблиця Lessons
CREATE TABLE Lessons (
    lesson_id SERIAL PRIMARY KEY,
    subject_id INT REFERENCES Subjects(subject_id) ON DELETE SET NULL,
    slot_id INT REFERENCES Lessons_Slots(slot_id) ON DELETE SET NULL,
    lecture_id INT REFERENCES Lectures(lecture_id) ON DELETE SET NULL
);

-- Таблиця Schedules
CREATE TABLE Schedules (
    schedule_id SERIAL PRIMARY KEY,
    lesson_id INT REFERENCES Lessons(lesson_id) ON DELETE CASCADE
);

-- Таблиця Tasks
CREATE TABLE Tasks (
    task_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES Users(user_id) ON DELETE CASCADE,
    description TEXT,
    title VARCHAR(255),
    deadline DATE,
    status VARCHAR(50),
    creationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблиця Comments
CREATE TABLE Comments (
    comment_id SERIAL PRIMARY KEY,
    task_id INT REFERENCES Tasks(task_id) ON DELETE CASCADE,
    description TEXT,
    creationDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Таблиця Categories
CREATE TABLE Categories (
    category_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

-- Таблиця Support_Tickets
CREATE TABLE Support_Tickets (
    ticket_id SERIAL PRIMARY KEY,
    category_id INT REFERENCES Categories(category_id) ON DELETE SET NULL,
    user_id INT REFERENCES Users(user_id) ON DELETE CASCADE,
    description TEXT,
    type VARCHAR(50)
);

-- Таблиця для зв’язку “Manipulate” між Users і Categories
CREATE TABLE User_Category_Manipulate (
    user_id INT REFERENCES Users(user_id) ON DELETE CASCADE,
    category_id INT REFERENCES Categories(category_id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, category_id)
);

-- Таблиця для зв’язку “Manipulate” між Users і Support_Tickets
CREATE TABLE User_Ticket_Manipulate (
    user_id INT REFERENCES Users(user_id) ON DELETE CASCADE,
    ticket_id INT REFERENCES Support_Tickets(ticket_id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, ticket_id)
);

-- Таблиця для зв’язку “Contains” між Users і Lessons (many-to-many)
CREATE TABLE User_Lessons (
    user_id INT REFERENCES Users(user_id) ON DELETE CASCADE,
    lesson_id INT REFERENCES Lessons(lesson_id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, lesson_id)
);
