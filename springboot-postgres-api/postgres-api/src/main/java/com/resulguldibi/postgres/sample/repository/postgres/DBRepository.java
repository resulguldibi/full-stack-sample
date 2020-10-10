package com.resulguldibi.postgres.sample.repository.postgres;

import com.resulguldibi.postgres.sample.client.postgres.interfaces.IDBClient;
import com.resulguldibi.postgres.sample.entity.SampleEntity;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

public class DBRepository implements IDBRepository {

    private IDBClient client;

    public DBRepository(IDBClient client){
        this.client = client;
    }

    @Override
    public List<SampleEntity> GetAllSamples() throws SQLException {
        ResultSet rs = this.client.Query("SELECT id,name from samples;");
        List<SampleEntity> list = new ArrayList<>();
        while (rs.next()) {
             list.add(new SampleEntity(rs.getInt(1),rs.getString(2)));
        }
        return list;
    }

    @Override
    public int AddNewSample(SampleEntity sample) throws SQLException {

        List<Object> parameters = new ArrayList<>();
        parameters.add(sample.getId());
        parameters.add(sample.getName());

        return this.client.Command("insert into samples(id,name) values(?,?);",parameters);
    }
}
