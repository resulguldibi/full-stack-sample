package com.resulguldibi.postgres.sample.entity;


import com.fasterxml.jackson.annotation.JsonProperty;

public class SelectionEntity {
    @JsonProperty("count")
    private int count;

    @JsonProperty("user_id")
    private String userId;

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }

    @JsonProperty("name")
    private String name;

    public SelectionEntity(){

    }

    public SelectionEntity(int count,String name,String userId){
        this.count = count;
        this.name = name;
        this.userId = userId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
}
