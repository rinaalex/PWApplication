# TestTz
Это тестовое задание кандидата на должность .Net C# developer, текст техзадания представлен в файле TestTz
## База данных
База данных состоит из трех таблиц: Users, Transfers и Operation.
Таблица User содержит информацию о пользователях. Таблица Transfers содержит информацию о переводе (кто, кому, когда и сколько). Итоговые балансы и суммы прихода/ухода хранятся в таблице Operation. Используя такую запись можно отследить транзакции при возникновении несостыковок.
## Структура приложения
Проект DataLayer содержит модели сущностей и контекст данных. 
Проект ServiceLayer содержит логику обработки данных.
Проект PwWebApp реализует web api. 
## Детали реализации
Фильтрация и сортировка списка транзакций созданы в ServiceLayer, но еще не реализованы в веб-приложении.
Копирование транзакции также создано в ServiceLayer, но еще не реализовано в веб-приложении.
