package com.resulguldibi.postgres.sample.client.cassandra.interfaces;

import com.datastax.driver.core.ResultSet;

public interface ICassandraSession {
    ResultSet Execute(String cqlQuery);
}
