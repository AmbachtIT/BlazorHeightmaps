# This script runs all tools that are needed during development


dotnet watch --no-hot-reload --project src\BlazorHeightmaps.Wasm.App\BlazorHeightmaps.Wasm.App.csproj

# .\dev\tailwindcss-windows-x64.exe -i ".\src\BlazorHeightmaps.Wasm.App\wwwroot\css\app.css" -o ".\src\BlazorHeightmaps.Wasm.App\wwwroot\css\app.min.css" --watch