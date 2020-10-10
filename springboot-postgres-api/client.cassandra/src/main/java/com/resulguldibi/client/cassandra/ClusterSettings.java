package com.resulguldibi.client.cassandra;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.PropertySource;

import javax.annotation.Resource;

@Configuration
@ConfigurationProperties(prefix = "mycluster")
@Resource
@PropertySource(value = "classpath:application.properties")

public class ClusterSettings {

    private String hosts;

    private int port;

    public String getHosts() {
        return hosts;
    }

    public void setHosts(String hosts) {
        this.hosts = hosts;
    }

    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }
}
