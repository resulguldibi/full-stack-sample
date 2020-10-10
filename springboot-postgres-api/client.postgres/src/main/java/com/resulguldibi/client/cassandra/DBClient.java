package com.resulguldibi.client.cassandra;

import com.resulguldibi.client.cassandra.interfaces.IDBClient;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

public class DBClient implements IDBClient {
    private Connection connection = null;
    public DBClient(Connection connection){
        this.connection = connection;
    }

    @Override
    public ResultSet Query(String query) throws SQLException {
        return this.connection.createStatement().executeQuery(query);
    }

    public int Command(String query, List<Object> parameters) throws SQLException {
        PreparedStatement pst = this.connection.prepareStatement(query);
        if(parameters != null && parameters.size() > 0){
            for (int i=0;i<parameters.size();i++){
                pst.setObject(i+1,parameters.get(i));
            }
        }
        int result = pst.executeUpdate();
        pst.close();
        return result;
    }
}
