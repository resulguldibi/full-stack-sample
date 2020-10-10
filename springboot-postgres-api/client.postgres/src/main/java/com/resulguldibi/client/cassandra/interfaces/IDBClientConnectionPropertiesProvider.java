package com.resulguldibi.client.cassandra.interfaces;

import java.util.Properties;

public interface IDBClientConnectionPropertiesProvider {

    Properties GetConnectionProperties(String user,String password,boolean useSsl);
}
