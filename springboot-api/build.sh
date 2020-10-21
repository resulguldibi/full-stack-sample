docker build -t resulguldibi/postgres-api .
docker-compose up -d
docker exec -it cassandra cqlsh -e "CREATE KEYSPACE my_keyspace WITH replication = {'class' : 'NetworkTopologyStrategy','my_datacenter' : 1};"
docker exec -it cassandra cqlsh -e "CREATE TABLE my_keyspace.my_table (id text primary key,count int,word text);"
docker exec -it cassandra cqlsh -e "insert into my_keyspace.my_table(id,count,word) values('1',10,'sample');"
docker exec -it postgres psql -U postgresuser -c "create database my_database;"
docker exec -it postgres psql -U postgresuser my_database -c "CREATE TABLE samples (id INTEGER PRIMARY KEY, name VARCHAR);"
docker exec -it postgres psql -U postgresuser my_database -c "insert into samples(id,name) values(1,'test');"

