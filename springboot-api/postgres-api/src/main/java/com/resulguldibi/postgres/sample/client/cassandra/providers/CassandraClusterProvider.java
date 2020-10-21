package com.resulguldibi.postgres.sample.client.cassandra.providers;

import com.resulguldibi.postgres.sample.client.cassandra.CassandraCluster;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraCluster;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraClusterProvider;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraConnectionInfoProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

@Repository
public class CassandraClusterProvider implements ICassandraClusterProvider {

    @Autowired
    private ICassandraConnectionInfoProvider cassandraConnectionInfoProvider;
    public CassandraClusterProvider(ICassandraConnectionInfoProvider cassandraConnectionInfoProvider)
    {
        this.cassandraConnectionInfoProvider = cassandraConnectionInfoProvider;
    }

    @Override
    public ICassandraCluster GetCassandraCluster(String id) {
        return new CassandraCluster(this.cassandraConnectionInfoProvider.GetCassandraConnectionInfo(id));
    }
}
