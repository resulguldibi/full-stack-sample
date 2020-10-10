package com.resulguldibi.client.cassandra.interfaces;

import com.datastax.driver.core.ResultSet;

public interface ICassandraClient{

    ResultSet execute(String cqlQuery);
}
