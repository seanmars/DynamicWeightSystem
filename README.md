# DynamicWeightSystem

## Requirements

1. .NET 9 SDK

## How to run WebApp

1. Setting up the environment variable `ASPNETCORE_ENVIRONMENT` to `Development` or `Production`:

    ```shell
    # bash
    export ASPNETCORE_ENVIRONMENT=Development
    # powershell
    $env:ASPNETCORE_ENVIRONMENT="Development"
    ```
2. (***OPTIONAL***) Setting up the configuration file `appsettings.{ENVIRONMENT}.json` if necessary:

    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "Data Source={DB_NAME}.sqlite;Cache=Shared;"
        }
    }
    ```

3. Setting up the database:

    ```shell
    dotnet ef database update
    ```

4. Running the web application, cd to the WebApp folder and run the following command:

    ```shell
    # run the backend and frontend
    dotnet run --launch-profile http
    # run the backend only
    dotnet run --launch-profile http-standalone
    ```

5. Access the application at `http://localhost:5231 (backend)`, `http://localhost:9284 (frontend)` or whatever you set up.
6. You can access the OpenAPI UI at `http://localhost:5231/scalar/v1`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
