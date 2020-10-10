package com.resulguldibi.postgres.sample.client.postgres.providers;

import com.resulguldibi.postgres.sample.client.postgres.DBClient;
import com.resulguldibi.postgres.sample.client.postgres.interfaces.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import java.sql.SQLException;
@Repository
public class DBClientProvider implements IDBClientProvider {

    @Autowired
    private IDBConnectionProvider postgresConnectionProvider;
    @Autowired
    private IDBClientConnectionPropertiesProvider postgresClientConnectionPropertiesProvider;
    @Autowired
    private IDBClientConnectionStringProvider postgresClientConnectionStringProvider;

    public DBClientProvider(IDBConnectionProvider postgresConnectionProvider, IDBClientConnectionPropertiesProvider postgresClientConnectionPropertiesProvider, IDBClientConnectionStringProvider postgresClientConnectionStringProvider){
        this.postgresConnectionProvider = postgresConnectionProvider;
        this.postgresClientConnectionPropertiesProvider = postgresClientConnectionPropertiesProvider;
        this.postgresClientConnectionStringProvider = postgresClientConnectionStringProvider;
    }

    @Override
    public IDBClient GetPostgresClient(String host, int port, String database, String user, String password, boolean useSsl) throws SQLException {

        return new DBClient(this.postgresConnectionProvider.GetConnection(this.postgresClientConnectionStringProvider.GetConnectionString(host,port,database),this.postgresClientConnectionPropertiesProvider.GetConnectionProperties(user,password,useSsl)));
    }
}
