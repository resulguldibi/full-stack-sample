package com.resulguldibi.client.cassandra.interfaces;

import org.springframework.stereotype.Repository;

@Repository
public interface ICassandraConnectionInfoProvider {
    ICassandraConnectionInfo GetCassandraConnectionInfo(String id);
}
