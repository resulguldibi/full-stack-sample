package com.resulguldibi.postgres.sample.client.cassandra.providers;

import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraCluster;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraClusterProvider;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraSession;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraSessionProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

@Repository
public class CassandraSessionProvider implements ICassandraSessionProvider {

    @Autowired
    private ICassandraClusterProvider cassandraClusterProvider;

    private ICassandraCluster cassandraCluster;
    public CassandraSessionProvider(ICassandraClusterProvider cassandraClusterProvider)
    {
        this.cassandraClusterProvider = cassandraClusterProvider;
    }

    @Override
    public ICassandraSession GetCassandraSession(String id, String keyspace) {
        if (cassandraCluster == null)
        {
            cassandraCluster = this.cassandraClusterProvider.GetCassandraCluster(id);
        }

        return this.cassandraCluster.Connect(keyspace);
    }
}
