#create postgres container using docker command

docker run -d --name my_postgres -v my_dbdata:/var/lib/postgresql/data -p 54320:5432 -e POSTGRES_PASSWORD=pwd123 -e POSTGRES_USER=postgresuser postgres:11

#create postgres container using docker-compose

docker-compose up -d

#connect postgres container

docker exec -it my_postgres psql -U postgresuser

#connect and create data in postgres container

docker exec -it my_postgres psql -U postgresuser -c "create database my_database"

#list databases in postgres

#connect postgres container

docker exec -it my_postgres psql -U postgresuser

#type \l to list database (\l stand for \list)

#type \c <<database>> to connect selected database

#create sample table in database

CREATE TABLE samples (id INTEGER PRIMARY KEY, name VARCHAR);

#type \dt to list tables

#select query from table

select * from samples;

#insert sample record to table

insert into samples(id,name) values(1,'test');



