package com.resulguldibi.client.cassandra.interfaces;

public interface ICassandraCluster {
    ICassandraSession Connect(String keyspace);
}
