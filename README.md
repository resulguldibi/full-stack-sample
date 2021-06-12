

1. When you display http://localhost:8080/index.html page, a socket connection is established via ws://localhost:8080/ws with data like below.

--socket message content
{"code":"connect","data":{"ip":"127.0.0.1","user":"test-user"}}

2. After scoket connection established, a synchronization message sent via http://localhost:8080/api/selections/synchronization with data like below.

--synchronization message content
{"user_id":"test-user"}

This message produced to Synchronizations topic in kafka.

 2.1 A go routine in go-service consumes messages from Synchronizations topic in kafka and get all selection count for that user via http://postgres-api:8080/selections and produces message to SynchronizationResults topic in kafka with this data.
 
 2.2 Background task in netcore-api consumes messages from SynchronizationResults topic in kafka and then find socket connection with user info and send this message to founded socket connection.

 
3. when you choose a,b,c or d from "selection results via socket" table, selection details sent via http://localhost:8080/api/selections/async with payload like below:

--payload
{"name":"d","id":"e0fcd4e5-bdc2-4f4e-f6e8-920bfc07804a","user_id":"test-user"}

4. Selection details are produced to Selections topic in kafka.

5. A go routine in go-service consumes messages from Selections topic and first inserts selection details selections table in cassandra (insert into selections (id, name) VALUES (?, ?)) and then select count for that selection (select count(1) as count from selections where name = ?). 

6. After getting final count for that selection, a go routine produces message to SelectionSummary topic (message content {"name":"d","count":10,"user_id":"test-user"}) and second go routine sends same data to http://postgres-api:8080/selections to updates count in selections_summary table (update selections_summary set count = ? where name = ?) in postgres.

7. Background task in netcore-api consumes messages from SelectionSummary topic in kafka and then find socket connection with user info and send this message to founded socket connection.


