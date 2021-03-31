
setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

cd /d D:\redis\

start redis-server
start "localhost:6000" redis-server --port 6000
start "localhost:6001" redis-server --port 6001
start "localhost:6002" redis-server --port 6002
