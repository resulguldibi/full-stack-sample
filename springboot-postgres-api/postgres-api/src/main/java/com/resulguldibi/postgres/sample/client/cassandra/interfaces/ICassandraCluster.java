package com.resulguldibi.postgres.sample.client.cassandra.interfaces;

public interface ICassandraCluster {
    ICassandraSession Connect(String keyspace);
}
