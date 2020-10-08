package com.resulguldibi.postgres.sample.entity;


import com.fasterxml.jackson.annotation.JsonProperty;

public class SampleEntity {
    @JsonProperty("id")
    private int id;

    @JsonProperty("name")
    private String name;

    public SampleEntity(){

    }

    public SampleEntity(int id,String name){
        this.id = id;
        this.name = name;
    }
    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
}
