package com.resulguldibi.postgres.sample.repository.cassandra;

import java.sql.SQLException;

public interface ICassandraRepository {
    String GetVersion() throws SQLException;

}
