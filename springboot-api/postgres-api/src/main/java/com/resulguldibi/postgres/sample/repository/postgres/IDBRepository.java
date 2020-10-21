package com.resulguldibi.postgres.sample.repository.postgres;

import com.resulguldibi.postgres.sample.entity.SampleEntity;
import com.resulguldibi.postgres.sample.entity.SelectionEntity;

import java.sql.SQLException;
import java.util.List;

public interface IDBRepository {

    List<SampleEntity> GetAllSamples() throws SQLException;

    int AddNewSample(SampleEntity sample) throws SQLException;

    int UpdateSelection(SelectionEntity selection) throws SQLException;

    List<SelectionEntity> GetAllSelections() throws SQLException;

    void Close() throws SQLException;
}
