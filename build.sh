docker build -t resulguldibi/cassandra-api .
docker-compose up -d
docker exec -it cassandra cqlsh -e "CREATE KEYSPACE my_keyspace WITH replication = {'class' : 'NetworkTopologyStrategy','my_datacenter' : 1};CREATE TABLE my_keyspace.my_table (id text primary key,count int,word text); insert into my_keyspace.my_table(id,count,word) values('1',10,'sample');"