package com.resulguldibi.client.cassandra.providers;

import com.resulguldibi.client.cassandra.interfaces.ICassandraCluster;
import com.resulguldibi.client.cassandra.interfaces.ICassandraClusterProvider;
import com.resulguldibi.client.cassandra.interfaces.ICassandraSession;
import com.resulguldibi.client.cassandra.interfaces.ICassandraSessionProvider;
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
