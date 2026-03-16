# השתמש ב-SDK של דוט-נט כדי לבנות את האפליקציה
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# העתק את קבצי הפרויקט ובצע Restore
COPY *.csproj ./
RUN dotnet restore

# העתק את שאר הקבצים ובנה את הפרויקט
COPY . ./
RUN dotnet publish -c Release -o out

# השתמש ב-Runtime קטן יותר להרצה
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# הגדרת הפורט ש-Render מצפה לו
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "TodoApi.dll"]