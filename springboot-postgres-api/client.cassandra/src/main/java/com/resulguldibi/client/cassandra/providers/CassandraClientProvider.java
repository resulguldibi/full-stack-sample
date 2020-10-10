package com.resulguldibi.client.cassandra.providers;

import com.resulguldibi.client.cassandra.CassandraClient;
import com.resulguldibi.client.cassandra.interfaces.ICassandraClient;
import com.resulguldibi.client.cassandra.interfaces.ICassandraClientProvider;
import com.resulguldibi.client.cassandra.interfaces.ICassandraSessionProvider;
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
