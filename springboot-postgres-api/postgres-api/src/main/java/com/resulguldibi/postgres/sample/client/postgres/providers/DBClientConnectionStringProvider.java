package com.resulguldibi.postgres.sample.client.postgres.providers;

import com.resulguldibi.postgres.sample.client.postgres.interfaces.IDBClientConnectionStringProvider;
import org.springframework.stereotype.Repository;

@Repository

public class DBClientConnectionStringProvider implements IDBClientConnectionStringProvider {
    @Override
    public String GetConnectionString(String host,int port,String database) {
        return String.format("jdbc:postgresql://%s:%d/%s",host,port,database);
    }
}
