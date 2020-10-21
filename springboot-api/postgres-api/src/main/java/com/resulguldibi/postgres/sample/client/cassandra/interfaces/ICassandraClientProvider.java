package com.resulguldibi.postgres.sample.client.cassandra.interfaces;

import org.springframework.stereotype.Repository;

@Repository
public interface ICassandraClientProvider {
    ICassandraClient GetCassandraClient(String id, String keyspace);
}
