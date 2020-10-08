package com.resulguldibi.postgres.sample.controller;

import com.resulguldibi.postgres.sample.entity.SampleEntity;
import com.resulguldibi.postgres.sample.repository.IDBRepository;
import com.resulguldibi.postgres.sample.repository.IDBRepositoryProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.sql.SQLException;
import java.util.List;

@RestController
public class SampleController {

    @Autowired
    IDBRepositoryProvider postgresRepositoryProvider;

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
}
