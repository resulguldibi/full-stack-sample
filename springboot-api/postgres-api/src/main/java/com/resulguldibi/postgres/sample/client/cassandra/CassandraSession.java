package com.resulguldibi.postgres.sample.client.cassandra;

import com.datastax.driver.core.ResultSet;
import com.datastax.driver.core.Session;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraSession;

public class CassandraSession implements ICassandraSession {
    private Session session;
    public CassandraSession(Session session)
    {
        this.session = session;
    }

    @Override
    public ResultSet Execute(String cqlQuery) {
        return this.session.execute(cqlQuery);
    }
}
