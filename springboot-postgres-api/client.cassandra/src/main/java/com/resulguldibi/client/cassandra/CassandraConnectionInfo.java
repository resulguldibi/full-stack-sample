package com.resulguldibi.client.cassandra;

import com.resulguldibi.client.cassandra.interfaces.ICassandraConnectionInfo;

public class CassandraConnectionInfo implements ICassandraConnectionInfo {

    private String[] hosts;
    private int port;
    public CassandraConnectionInfo(String[] hosts, int port)
    {
        this.hosts = hosts;
        this.port = port;
    }

    @Override
    public String[] GetHosts() {
        return this.hosts;
    }

    @Override
    public int GetPort() {
        return this.port;
    }
}
