package com.resulguldibi.postgres.sample.client.cassandra.providers;

import com.resulguldibi.postgres.sample.client.cassandra.CassandraClient;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraClient;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraClientProvider;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraSessionProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

@Repository
public class CassandraClientProvider implements ICassandraClientProvider {

    @Autowired
    private ICassandraSessionProvider cassandraSessionProvider;

    public CassandraClientProvider(ICassandraSessionProvider cassandraSessionProvider)
    {
        this.cassandraSessionProvider = cassandraSessionProvider;
    }

    @Override
    public ICassandraClient GetCassandraClient(String id, String keyspace) {
        return new CassandraClient(this.cassandraSessionProvider.GetCassandraSession(id, keyspace));
    }
}
