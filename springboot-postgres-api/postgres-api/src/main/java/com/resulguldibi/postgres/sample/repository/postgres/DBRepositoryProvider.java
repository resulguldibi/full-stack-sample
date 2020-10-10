package com.resulguldibi.postgres.sample.repository.postgres;

import com.resulguldibi.postgres.sample.client.postgres.interfaces.IDBClientProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import java.sql.SQLException;

@Repository
public class DBRepositoryProvider implements IDBRepositoryProvider {

    @Autowired
    IDBClientProvider postgresClientProvider;
    @Override
    public IDBRepository GetPostgresRepository() throws SQLException {
        return new DBRepository(postgresClientProvider.GetPostgresClient("172.24.0.1",5432,"my_database","postgresuser","pwd123",false));
    }
}
