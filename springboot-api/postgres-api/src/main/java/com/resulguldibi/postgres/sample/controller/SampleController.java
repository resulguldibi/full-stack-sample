package com.resulguldibi.postgres.sample.controller;

import com.resulguldibi.postgres.sample.entity.SampleEntity;
import com.resulguldibi.postgres.sample.repository.cassandra.ICassandraRepository;
import com.resulguldibi.postgres.sample.repository.cassandra.ICassandraRepositoryProvider;
import com.resulguldibi.postgres.sample.repository.postgres.IDBRepository;
import com.resulguldibi.postgres.sample.repository.postgres.IDBRepositoryProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.sql.SQLException;
import java.util.List;

@RestController
public class SampleController {

    @Autowired
    IDBRepositoryProvider postgresRepositoryProvider;

    @Autowired
    ICassandraRepositoryProvider cassandraRepositoryProvider;

    @GetMapping(path = "/entities", produces = "application/json")
    public List<SampleEntity> getAllEntities() throws SQLException {
        IDBRepository repository = postgresRepositoryProvider.GetPostgresRepository();
        return repository.GetAllSamples();
    }

    @PostMapping(path = "/entities", consumes = "application/json", produces = "application/json")
    public int addEmployee(@RequestBody SampleEntity entity) throws SQLException {
        IDBRepository repository = postgresRepositoryProvider.GetPostgresRepository();
        return repository.AddNewSample(entity);
    }

    @GetMapping(path = "/cassandra/version", produces = "application/json")
    public String getCassandraVersion() throws SQLException {
        ICassandraRepository cassandraRepository = cassandraRepositoryProvider.GetCassandraRepository();
        return cassandraRepository.GetVersion();
    }

}
