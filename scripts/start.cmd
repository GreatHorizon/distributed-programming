cd ..\Valuator\
start "first_app_exemplar" dotnet run --no-build --urls "http://localhost:5001"
start "second_app_exemplar" dotnet run --no-build --urls "http://localhost:5002"

cd ..\nginx\
start nginx

cd ..\RankCalculator\
start "first_consumer" dotnet run --no-build
start "second_consumer" dotnet run --no-build

