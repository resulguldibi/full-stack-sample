package com.resulguldibi.client.cassandra.interfaces;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

public interface IDBClient {

    ResultSet Query(String query) throws SQLException;
    int Command(String query, List<Object> parameters) throws SQLException;
}
