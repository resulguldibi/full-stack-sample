package com.resulguldibi.postgres.sample.client.postgres.providers;

import com.resulguldibi.postgres.sample.client.postgres.interfaces.IDBClientConnectionPropertiesProvider;
import org.springframework.stereotype.Repository;

import java.util.Properties;

@Repository

public class DBClientConnectionPropertiesProvider implements IDBClientConnectionPropertiesProvider {
    @Override
    public Properties GetConnectionProperties(String user, String password, boolean useSsl) {
        Properties props = new Properties();
        props.setProperty("user",user);
        props.setProperty("password",password);
        props.setProperty("ssl",String.valueOf(useSsl));
        return props;
    }
}
