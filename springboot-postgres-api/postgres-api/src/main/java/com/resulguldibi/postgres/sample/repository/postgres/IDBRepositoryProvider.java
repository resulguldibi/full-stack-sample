package com.resulguldibi.postgres.sample.repository.postgres;

import java.sql.SQLException;

public interface IDBRepositoryProvider {

    IDBRepository GetPostgresRepository() throws SQLException;
}
