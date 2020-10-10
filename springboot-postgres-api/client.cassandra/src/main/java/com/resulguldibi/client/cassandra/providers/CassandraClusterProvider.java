package com.resulguldibi.client.cassandra.providers;

import com.resulguldibi.client.cassandra.CassandraCluster;
import com.resulguldibi.client.cassandra.interfaces.ICassandraCluster;
import com.resulguldibi.client.cassandra.interfaces.ICassandraClusterProvider;
import com.resulguldibi.client.cassandra.interfaces.ICassandraConnectionInfoProvider;
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
