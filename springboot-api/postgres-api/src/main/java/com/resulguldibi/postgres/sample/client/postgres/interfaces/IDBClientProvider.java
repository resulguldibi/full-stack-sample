package com.resulguldibi.postgres.sample.client.postgres.interfaces;

import java.sql.SQLException;

public interface IDBClientProvider {

    IDBClient GetPostgresClient(String host, int port, String database, String user, String password, boolean useSsl) throws SQLException;
}
