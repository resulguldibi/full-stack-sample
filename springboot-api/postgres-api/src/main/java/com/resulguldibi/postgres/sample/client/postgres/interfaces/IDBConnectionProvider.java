package com.resulguldibi.postgres.sample.client.postgres.interfaces;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.Properties;

public interface IDBConnectionProvider {
    Connection GetConnection(String url, Properties properties) throws SQLException;
}
