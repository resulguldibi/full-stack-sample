package com.resulguldibi.postgres.sample;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.security.servlet.SecurityAutoConfiguration;

@SpringBootApplication(exclude = SecurityAutoConfiguration.class)

public class PostgreSampleApplication {
    public static void main(String[] args) {
        SpringApplication.run(PostgreSampleApplication.class, args);
    }
}
