package com.resulguldibi.postgres.sample.client;

import java.util.Properties;

public interface IDBClientConnectionPropertiesProvider {

    Properties GetConnectionProperties(String user,String password,boolean useSsl);
}
