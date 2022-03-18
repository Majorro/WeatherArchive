# Weather Archive

This is an ASP.NET Core MVC app that allows upload and explore weather archives.

## Features

- Idempotent upload of single/multiple .xlsx/.xls files
- Explore all of uploaded with filtration and pagination
- File/Data validations
- Русский язык (anti-feature:/ need to add localization)

## Technologies used

1. .NET 6
2. C# 10
3. ASP.NET Core 6 MVC
4. Entity Framework Core 6 with Code-First approach
5. [Entity Framework Extensions](https://entityframework-extensions.net/) - library for database operations with a large number of entities
6. NPOI - for excel parsing
7. HTML5
8. CSS3
9. jQuery 3
10. Docker - for fun:)

Also there's bootstrap, but it's from project template

## Prerequisites

1. [Docker](https://docs.docker.com/get-docker/) to build and run the app locally.

## Building and running

1. Simply run the following command from the root of the repository. You can also add `-d` key to run it in the background:
```bash
docker compose up --build
```
2. The running web app can be found at http://localhost:8080/

## Usage demonstration

![demo](https://s10.gifyu.com/images/demo88fee9982f8333d5.gif)

## License

[MIT](LICENSE)
