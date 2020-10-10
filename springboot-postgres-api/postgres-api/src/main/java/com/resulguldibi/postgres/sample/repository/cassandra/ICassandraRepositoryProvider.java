package com.resulguldibi.postgres.sample.repository.cassandra;

import java.sql.SQLException;

public interface ICassandraRepositoryProvider {

    ICassandraRepository GetCassandraRepository() throws SQLException;
}
