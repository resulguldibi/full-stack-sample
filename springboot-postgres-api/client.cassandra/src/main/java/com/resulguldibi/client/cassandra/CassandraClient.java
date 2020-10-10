package com.resulguldibi.client.cassandra;

import com.datastax.driver.core.ResultSet;
import com.resulguldibi.client.cassandra.interfaces.ICassandraClient;
import com.resulguldibi.client.cassandra.interfaces.ICassandraSession;

public class CassandraClient implements ICassandraClient {
    private ICassandraSession cassandraSession;

    public  CassandraClient( ICassandraSession cassandraSession){
        this.cassandraSession= cassandraSession;
    }

    @Override
    public ResultSet execute(String cqlQuery) {
        return this.cassandraSession.Execute(cqlQuery);
    }
}
