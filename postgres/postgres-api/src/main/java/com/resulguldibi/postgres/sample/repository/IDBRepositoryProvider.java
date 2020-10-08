package com.resulguldibi.postgres.sample.repository;

import java.sql.SQLException;

public interface IDBRepositoryProvider {

    IDBRepository GetPostgresRepository() throws SQLException;
}
