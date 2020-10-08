package com.resulguldibi.postgres.sample.client;

public interface IDBClientConnectionStringProvider {

    String GetConnectionString(String host,int port,String database);
}
