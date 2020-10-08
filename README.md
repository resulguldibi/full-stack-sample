
--create topic in kafka
./kafka-topics.sh --zookeeper 172.26.0.1:2181 --topic Test_Topic2 --partitions 1 --replication-factor 1 --create