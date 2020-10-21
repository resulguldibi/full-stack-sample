package com.resulguldibi.postgres.sample.client.cassandra.providers;

import com.resulguldibi.postgres.sample.client.cassandra.CassandraConnectionInfo;
import com.resulguldibi.postgres.sample.client.cassandra.ClusterSettings;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraConnectionInfo;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraConnectionInfoProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

@Repository
public class CassandraConnectionInfoProvider implements ICassandraConnectionInfoProvider {

    @Autowired
    private ClusterSettings settings;

    @Override
    public ICassandraConnectionInfo GetCassandraConnectionInfo(String id) {

        return new CassandraConnectionInfo(settings.getHosts().split(","), settings.getPort());
    }
}
