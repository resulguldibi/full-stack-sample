# cassandra-api

 cassandra-api contains example about accessing cassandra database using .net core.

- change directory to cassandra-api project directory. <br/>
  cd "cassandra-api"

- run build.sh. running build.sh starts operations below:
    - builds cassandra-api docker image (docker build -t resulguldibi/cassandra-api .)
    - starts cassandra-api container and cassandra container (run docker-compose up -d)
    - creates keyspace and table in cassandra container and adds sample record to created table.
        -  creates "my_keyspace" keyspace.("CREATE KEYSPACE my_keyspace WITH replication = {'class' : 'NetworkTopologyStrategy','my_datacenter' : 1};")
        - creates "my_table" table in "my_keyspace" keyspace. ("CREATE TABLE my_keyspace.my_table (id text primary key,count int,word text);")
        - inserts sample record to "my_table" table. ("insert into my_keyspace.my_table(id,count,word) values('1',10,'sample');")

- get sample record from cassandra. (curl -v http://localhost:8080/api/samples)