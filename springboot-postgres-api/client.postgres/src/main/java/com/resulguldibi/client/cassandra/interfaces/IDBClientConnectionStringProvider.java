package com.resulguldibi.client.cassandra.interfaces;

public interface IDBClientConnectionStringProvider {

    String GetConnectionString(String host,int port,String database);
}
