package com.resulguldibi.client.cassandra.providers;

import com.resulguldibi.client.cassandra.interfaces.IDBConnectionProvider;
import org.springframework.stereotype.Repository;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.Properties;

@Repository

public class DBConnectionProvider implements IDBConnectionProvider {
    @Override
    public Connection GetConnection(String url, Properties properties) throws SQLException {
        return DriverManager.getConnection(url, properties);
    }
}
