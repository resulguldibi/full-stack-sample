package com.resulguldibi.postgres.sample.repository.cassandra;

import com.datastax.driver.core.ResultSet;
import com.datastax.driver.core.Row;
import com.resulguldibi.postgres.sample.client.cassandra.interfaces.ICassandraClient;


import java.sql.SQLException;

public class CassandraRepository implements ICassandraRepository{

    private ICassandraClient client;

    public CassandraRepository(ICassandraClient client){
        this.client = client;
    }

    @Override
    public String GetVersion() throws SQLException {
        ResultSet rs = this.client.execute("select release_version from system.local");

        Row row = rs.one();
        return row.getString("release_version");

    }
}
