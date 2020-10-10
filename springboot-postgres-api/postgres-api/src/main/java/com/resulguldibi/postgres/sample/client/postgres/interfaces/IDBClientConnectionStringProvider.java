package com.resulguldibi.postgres.sample.client.postgres.interfaces;

public interface IDBClientConnectionStringProvider {

    String GetConnectionString(String host,int port,String database);
}
