cd ..\Valuator\
start "first_app_exemplar" dotnet run --urls "http://localhost:5001"
start "second_app_exemplar" dotnet run --urls "http://localhost:5002"

cd ..\nginx\
start nginx

cd ..\RankCalculator\
start "first_consumer" dotnet run
start "second_consumer" dotnet run


cd ..\EventLogger\
start "first_logger" dotnet run
start "second_logger" dotnet run


