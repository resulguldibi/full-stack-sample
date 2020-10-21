package com.resulguldibi.postgres.sample.controller;

import com.resulguldibi.postgres.sample.entity.SelectionEntity;
import com.resulguldibi.postgres.sample.repository.postgres.IDBRepository;
import com.resulguldibi.postgres.sample.repository.postgres.IDBRepositoryProvider;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.sql.SQLException;
import java.util.List;

@RestController
public class SelectionController {

    @Autowired
    IDBRepositoryProvider postgresRepositoryProvider;

    @PutMapping(path = "/selections", consumes = "application/json", produces = "application/json")
    public int updateSelections(@RequestBody SelectionEntity selection) throws SQLException {
        IDBRepository repository = null;

        try{
            repository = postgresRepositoryProvider.GetPostgresRepository();
            return repository.UpdateSelection(selection);
        }
        finally {
            if(repository != null){
                repository.Close();
            }
        }
    }

    @GetMapping(path = "/selections", produces = "application/json")
    public List<SelectionEntity> getAllSelections() throws SQLException {
        IDBRepository repository = null;

        try{
            repository = postgresRepositoryProvider.GetPostgresRepository();
            return repository.GetAllSelections();
        }
        finally {
            if(repository != null){
                repository.Close();
            }
        }
    }
}
