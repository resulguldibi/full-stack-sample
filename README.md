
--create topic in kafka
./kafka-topics.sh --zookeeper 172.26.0.1:2181 --topic Test_Topic2 --partitions 1 --replication-factor 1 --create

--insert sample message 

./kafka-console-producer.sh --broker-list localhost:9092 --topic Test_Topic2


--create table in cassandra (in my_keyspace keyspace)
CREATE TABLE my_keyspace.selections (id text,name text, PRIMARY KEY (name,id));
CREATE INDEX on my_keyspace.selections(name);



--send selection 

curl -kv http://localhost:8080/api/selections/async -d '{"id":"3","name":"c"}' -H 'Content-Type: application/json'