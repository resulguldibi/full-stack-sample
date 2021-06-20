# full-stack-sample

This sample consist of components below:

 * ngnix-ui
 * netcore-api
 * springboot-api
 * go-service
 * kafka
 * zookeeper
 * cassandra
 * postgres


# ngnix-ui
ngnix-ui container hosts index.html file. Web socket connection is established via index.html. User selection's total count is calculated and displayed using two different approaches:

 * <strong>via js</strong> <br />
   Simply, counter in javascript in incremented for that selection. (no backend integration)
 * <strong>via web socket</strong> <br/>
   Long journey :) Tried to be explained in "detailed flow of selection presentation" section.

# netcore-api
netcore-api produces message to Selections topic in kafka for user selection in nginix-ui and produces message to Synchronizations topic in kafka to get user selection's current count. Consumes messages from SelectionSummary and SynchronizationResults topics in kafka and produces message to client via socket connection.

# springboot-api
springboot-api exposes api to store user selection's count details to postgres and read user selection details from postgres.

# go-service
go-service consumes messages from Synchronizations topic in kafka and calls api that exposed by springboot-api to get user selection detail. After that, produces message to SynchronizationResults topic in kafka using this data.

go-service also consumes messages from Selections topic in kafka and inserts selection details to cassandra then get all selection detail for that user from cassandra. Produces a message to SelectionSummary topic in kafka using this data.

# kafka
kafka stores messages which produced by applications. <br/>
to create "Selections","SelectionSummary","Synchronizations" and "SynchronizationResults" topics with 1 partition and 1 replication factor in kafka:<br/>
(kafka-topics.sh file takes place in kafka container's /opt/kafka/bin path) <br/>

<pre><code>kafka-topics.sh --zookeeper zookeeper:2181 --topic Selections --partitions 1 --replication-factor 1 --create</code></pre><br/>
<pre><code>kafka-topics.sh --zookeeper zookeeper:2181 --topic SelectionSummary --partitions 1 --replication-factor 1 --create</code></pre><br/>
<pre><code>kafka-topics.sh --zookeeper zookeeper:2181 --topic Synchronizations --partitions 1 --replication-factor 1 --create</code></pre><br/>
<pre><code>kafka-topics.sh --zookeeper zookeeper:2181 --topic SynchronizationResults --partitions 1 --replication-factor 1 --create</code></pre><br/>
# zookeeper
kafka needs zookeeper for some internal processes. (leader election, cluster monitoring etc.)

# cassandra
cassandra stores all selection data in selections table (in selections_keyspace keyspace)

to create keyspace called "selections_keyspace" in cassandra: <br/>
<pre><code>CREATE KEYSPACE selections_keyspace WITH replication = {'class' : 'NetworkTopologyStrategy','my_datacenter' : 1};</code></pre>

to create table called "selections" in "selections_keyspace" keyspace: <br/>
<pre><code>CREATE TABLE selections_keyspace.selections (id text, name text, PRIMARY KEY (id, name));</code></pre>

to create index for "selections" table on "name" column: <br/>
<pre><code>CREATE INDEX on selections_keyspace.selections(name);</code></pre>


# postgres
postgres stores summary selection data in selections_summary table (in selections_database database)

to create database in postgres: <br/>
<pre><code>CREATE DATABASE selections_database;</code></pre>

to create table called "selections_summary" in "selections_database" database: <br/>
<pre><code>CREATE TABLE selections_summary (name VARCHAR, count INTEGER);</code></pre>

# detailed flow of selection presentation

<strong>1.</strong> When you display http://localhost:8081/index.html page, a socket connection is established via ws://localhost:8080/ws with data below.

socket message content <br/>
<pre><code>{"code":"connect","data":{"ip":"127.0.0.1","user":"test-user"}}</code></pre>

<strong>2.</strong> After scoket connection established, a synchronization message is sent via http://localhost:8080/api/selections/synchronization with data below.

synchronization message content
<pre><code>{"user_id":"test-user"}</code></pre>

This message is produced to Synchronizations topic in kafka.

   * A go routine in go-service consumes messages from Synchronizations topic in kafka and get all selection count for that user via http://postgres-api:8080/selections and produces message to SynchronizationResults topic in kafka with this data.

   * Background task in netcore-api consumes messages from SynchronizationResults topic in kafka and then finds socket connection with user info and sends this message to founded socket connection.


<strong>3.</strong> When you choose a,b,c or d from "selection results via socket" table, selection details is sent via http://localhost:8080/api/selections/async with payload below:

payload
<pre><code>{"name":"d","id":"e0fcd4e5-bdc2-4f4e-f6e8-920bfc07804a","user_id":"test-user"}</code></pre>

<strong>4.</strong> Selection details are produced to Selections topic in kafka.

<strong>5.</strong> A go routine in go-service consumes messages from Selections topic and first inserts selection details to selections table in cassandra (insert into selections (id, name) VALUES (?, ?)) and then fetches count for that selection (select count(1) as count from selections where name = ?).

<strong>6.</strong> After getting final count for that selection, a go routine produces message to SelectionSummary topic (message content {"name":"d","count":10,"user_id":"test-user"}) and second go routine sends same data to http://postgres-api:8080/selections to update count in selections_summary table (update selections_summary set count = ? where name = ?) in postgres.

<strong>7.</strong> Background task in netcore-api consumes messages from SelectionSummary topic in kafka and then findx socket connection with user info and sendx this message to founded socket connection.
