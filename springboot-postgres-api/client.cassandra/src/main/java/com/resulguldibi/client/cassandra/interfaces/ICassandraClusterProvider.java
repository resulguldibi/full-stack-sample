package com.resulguldibi.client.cassandra.interfaces;

import org.springframework.stereotype.Repository;

@Repository
public interface ICassandraClusterProvider {
    ICassandraCluster GetCassandraCluster(String id);
}
