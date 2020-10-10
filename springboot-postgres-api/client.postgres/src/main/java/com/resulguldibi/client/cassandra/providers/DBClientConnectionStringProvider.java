package com.resulguldibi.client.cassandra.providers;

import com.resulguldibi.client.cassandra.interfaces.IDBClientConnectionStringProvider;
import org.springframework.stereotype.Repository;

@Repository

public class DBClientConnectionStringProvider implements IDBClientConnectionStringProvider {
    @Override
    public String GetConnectionString(String host,int port,String database) {
        return String.format("jdbc:postgresql://%s:%d/%s",host,port,database);
    }
}
