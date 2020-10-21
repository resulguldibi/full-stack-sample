docker build -t resulguldibi/netcore-api netcore-api
docker build -t resulguldibi/full-stack-sample/go-service go-service
docker build -t resulguldibi/postgres-api springboot-api

docker-compose up -d

#create topic in kafka
docker exec -it kafka /opt/kafka/bin/kafka-topics.sh --zookeeper zookeeper:2181 --topic Selections --partitions 1 --replication-factor 1 --create
docker exec -it kafka /opt/kafka/bin/kafka-topics.sh --zookeeper zookeeper:2181 --topic SelectionSummary --partitions 1 --replication-factor 1 --create
docker exec -it kafka /opt/kafka/bin/kafka-topics.sh --zookeeper zookeeper:2181 --topic Synchronizations --partitions 1 --replication-factor 1 --create
docker exec -it kafka /opt/kafka/bin/kafka-topics.sh --zookeeper zookeeper:2181 --topic SynchronizationResults --partitions 1 --replication-factor 1 --create

#create keyspace and table in cassandra. insert sample record to cassandra table.
docker exec -it cassandra cqlsh -e "CREATE KEYSPACE selections_keyspace WITH replication = {'class' : 'NetworkTopologyStrategy','my_datacenter' : 1};"
docker exec -it cassandra cqlsh -e "CREATE TABLE selections_keyspace.selections (id text, name text, PRIMARY KEY (id, name));"
docker exec -it cassandra cqlsh -e "CREATE INDEX on selections_keyspace.selections(name);"
docker exec -it cassandra cqlsh -e "INSERT INTO selections_keyspace.my_table(id,count,word) VALUES('1',10,'sample');"

#creare database and table in postgres. insert sample record to postgres table
docker exec -it postgres psql -U postgresuser -c "create database selections_database;"
docker exec -it postgres psql -U postgresuser selections_database -c "CREATE TABLE selections_summary (name VARCHAR, count INTEGER);"
docker exec -it postgres psql -U postgresuser selections_database -c "INSERT INTO selections_summary(name,count) values('a', 0);"
docker exec -it postgres psql -U postgresuser selections_database -c "INSERT INTO selections_summary(name,count) values('b', 0);"
docker exec -it postgres psql -U postgresuser selections_database -c "INSERT INTO selections_summary(name,count) values('c', 0);"
docker exec -it postgres psql -U postgresuser selections_database -c "INSERT INTO selections_summary(name,count) values('d', 0);"
