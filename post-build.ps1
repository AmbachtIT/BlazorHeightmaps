# This script is executed in the post-build event

# Build tailwind stuff
.\dev\tailwindcss-windows-x64.exe -i ".\src\BlazorHeightmaps.Wasm.App\wwwroot\css\app.css" -o ".\src\BlazorHeightmaps.Wasm.App\wwwroot\css\app.min.css"