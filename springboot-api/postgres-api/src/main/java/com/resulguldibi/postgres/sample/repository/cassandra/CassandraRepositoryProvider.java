package com.resulguldibi.postgres.sample.repository.cassandra;

import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraClientProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import java.sql.SQLException;
@Repository
public class CassandraRepositoryProvider implements ICassandraRepositoryProvider{

    @Autowired
    ICassandraClientProvider cassandraClientProvider;

    @Override
    public ICassandraRepository GetCassandraRepository() throws SQLException {
        return new CassandraRepository(cassandraClientProvider.GetCassandraClient("my_cluster","my_keyspace"));
    }


}
