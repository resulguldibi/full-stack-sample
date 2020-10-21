package main

import (
	"encoding/json"
	"fmt"
	"github.com/confluentinc/confluent-kafka-go/kafka"
	"github.com/resulguldibi/http-client/entity"
	"github.com/resulguldibi/http-client/factory"
	"net/http"
	"os"
	"os/signal"
	"strconv"
	"sync"
	"syscall"
	"time"

	"github.com/gocql/gocql"
	"github.com/labstack/echo"
	"github.com/resulguldibi/consumer-dispatcher/consumer"
	"github.com/resulguldibi/consumer-dispatcher/dispatcher"
	"github.com/resulguldibi/consumer-dispatcher/model"
	"github.com/resulguldibi/consumer-dispatcher/producer"
	"github.com/resulguldibi/consumer-dispatcher/server"
	"github.com/resulguldibi/consumer-dispatcher/worker"
)

var consumerMaxWorker, consumerMaxQueue int
var producerMaxWorker, producerMaxQueue int
var cassandraJobDispatcher dispatcher.IDispatcher
var postgresApiJobDispatcher dispatcher.IDispatcher
var kafkaProducerSelectionSummaryJobDispatcher dispatcher.IDispatcher
var kafkaProducerSynchronizationResultJobDispatcher dispatcher.IDispatcher

var consumerSelectionTopic, consumerSynchronizationTopic, producerSelectionSummaryTopic, producerSynchronizationResultTopic, broker, selectionConsumerGroup, synchronizationConsumerGroup string

//region kafka consumer
var kafkaConsumerProvider consumer.IKafkaConsumerProvider
var kafkaSelectionConsumer consumer.IKafkaConsumer
var kafkaSynchronizationConsumer consumer.IKafkaConsumer
var kafkaConsumerSelectionMessageChannel chan interface{}
var kafkaConsumerSelectionErrorChannel chan interface{}
var kafkaConsumerSelectionIgnoreChannel chan interface{}
var kafkaConsumerSelectionSignalChannel chan os.Signal



var kafkaConsumerSynchronizationMessageChannel chan interface{}
var kafkaConsumerSynchronizationErrorChannel chan interface{}
var kafkaConsumerSynchronizationIgnoreChannel chan interface{}
var kafkaConsumerSynchronizationSignalChannel chan os.Signal

var kafkaConsumerProviderError error
var consumingLoop bool

//endregion

//region kafka producer
var kafkaProducerProvider producer.IKafkaProducerProvider
var kafkaProducer producer.IKafkaProducer
var kafkaProducerMessageChannel chan interface{}
var kafkaProducerErrorChannel chan interface{}
var kafkaProducerSignalChannel chan os.Signal
var kafkaProducerProviderError error

//endregion
//region cassandra cassandraCluster

var cassandraCluster *gocql.ClusterConfig
var cassandraSession *gocql.Session
var cassandraSessionError error

//endregion

//region httpclient

var httpClientFactory factory.IHttpClientFactory
var httpClient entity.IHttpClient

//endregion

type Selection struct {
	Id     string `json:"id"`
	Name   string `json:"name"`
	UserId string `json:"user_id"`
}

type SelectionSynchronization struct {
	Name   string `json:"name"`
	UserId string `json:"user_id"`
}

type SelectionSummmary struct {
	Name   string `json:"name"`
	Count  int    `json:"count"`
	UserId string `json:"user_id"`
}

func init() {

	/*
		--create "Test_Topic" in kafka container
		./kafka-topics.sh --zookeeper zookeeper:2181 --consumerSelectionTopic Test_Topic --partitions 1 -replication-factor 1 --create
		--add sample message to Test_Topic
		./kafka-console-producer.sh --broker-list localhost:9092 --consumerSelectionTopic Test_Topic
	*/

	//consumerMaxWorker, _ = strconv.Atoi(os.Getenv("MAX_WORKERS"))
	//consumerMaxQueue, _ = strconv.Atoi(os.Getenv("MAX_QUEUE"))

	consumerMaxWorker = 100
	consumerMaxQueue = 1000

	producerMaxWorker = 5
	producerMaxQueue = 100
	consumerSelectionTopic = "Selections"
	consumerSynchronizationTopic = "Synchronizations"
	producerSelectionSummaryTopic = "SelectionSummary"
	producerSynchronizationResultTopic = "SynchronizationResults"
	broker = "kafka:9092"
	selectionConsumerGroup = "test-selectionConsumerGroup"
	synchronizationConsumerGroup = "test-synchronizationConsumerGroup"

	//region http client

	httpClientFactory = factory.NewHttpClientFactory()
	httpClient = httpClientFactory.GetHttpClient()
	//endregion

	//region cassandra cluster

	cassandraCluster = gocql.NewCluster("192.168.48.1")
	cassandraCluster.Port = 9042
	cassandraCluster.Keyspace = "selections_keyspace"
	cassandraCluster.Consistency = gocql.Quorum
	cassandraSession, cassandraSessionError = cassandraCluster.CreateSession()

	if cassandraSessionError != nil {
		panic(cassandraSessionError)
	}
	//endregion

	cassandraJobDispatcher = dispatcher.NewDispatcher("cassandra-add-selection-dispatcher", consumerMaxWorker, consumerMaxQueue, func(worker worker.IWorker, job model.IJob) {
		defer func() {
			if r := recover(); r != nil {
				fmt.Println("Recovered in Consumer Task", r)
			}

			job.GetIsCompletedChannel() <- true
		}()

		fmt.Println(fmt.Sprintf("consumer workers %d is processing job : %v", worker.GetId(), job))

		//region insert record to cassandra
		var selection = Selection{}

		if err := json.Unmarshal(job.GetData().([]byte), &selection); err != nil {
			panic(err)
		}

		if err := cassandraSession.Query(`insert into selections (id, name) VALUES (?, ?)`,
			selection.Id, selection.Name).Exec(); err != nil {
			panic(err)
		}

		//endregion

		//region read all selection and count
		var count int

		//get selection count with name
		if err := cassandraSession.Query(`select count(1) as count from selections where name = ?`,
			selection.Name).Consistency(gocql.One).Scan(&count); err != nil {
			panic(err)
		}
		//endregion

		var selectionSummary = &SelectionSummmary{
			Name:   selection.Name,
			Count:  count,
			UserId: selection.UserId,
		}

		wg := sync.WaitGroup{}
		wg.Add(2)

		//region send selection summary to kafka

		go func(_selectionSummary *SelectionSummmary) {
			defer func() {
				if r := recover(); r != nil {
					fmt.Println("Recovered in SelectionSummmaryJob Task", r)
				}
				wg.Done()
			}()

			selectionSummmaryBytes, err := json.Marshal(_selectionSummary)

			if err != nil {
				panic(err)
			}

			message := &consumer.CustomKafkaMessage{Value: selectionSummmaryBytes, Partition: kafka.PartitionAny, Topic: producerSelectionSummaryTopic}
			selectionSummmaryJob := &model.Job{Data: message, IsCompletedChannel: make(chan bool)}
			kafkaProducerSelectionSummaryJobDispatcher.GetJobQueueChannel() <- selectionSummmaryJob

			_, ok := <-selectionSummmaryJob.IsCompletedChannel
			if ok {
				close(selectionSummmaryJob.IsCompletedChannel)
			}
		}(selectionSummary)

		//endregion

		//region send selection summary postgres-api

		go func(_selectionSummary *SelectionSummmary) {

			defer func() {
				if r := recover(); r != nil {
					fmt.Println("Recovered in SendSelectionSummmaryToPostgreJob Task", r)
				}
				wg.Done()
			}()
			httpClient = httpClientFactory.GetHttpClient()
			httpClient.PutJson("http://postgres-api:8080/selections", _selectionSummary)
		}(selectionSummary)

		//endregion

		wg.Wait()

	})

	postgresApiJobDispatcher = dispatcher.NewDispatcher("postgres-api-synchronize-selection-dispatcher", consumerMaxWorker, consumerMaxQueue, func(worker worker.IWorker, job model.IJob) {
		defer func() {
			if r := recover(); r != nil {
				fmt.Println("Recovered in Consumer Task", r)
			}

			job.GetIsCompletedChannel() <- true
		}()

		fmt.Println(fmt.Sprintf("consumer workers %d is processing job : %v", worker.GetId(), job))

		var selectionSynchronization = SelectionSynchronization{}

		if err := json.Unmarshal(job.GetData().([]byte), &selectionSynchronization); err != nil {
			panic(err)
		}

		var selections []SelectionSummmary
		httpClient = httpClientFactory.GetHttpClient()
		err := httpClient.Get("http://postgres-api:8080/selections").EndStruct(&selections)

		if err != nil {
			panic(err)
		}

		for i, _ := range selections {
			selections[i].UserId = selectionSynchronization.UserId
		}

		selectionsBytes, err := json.Marshal(selections)

		if err != nil {
			panic(err)
		}

		message := &consumer.CustomKafkaMessage{Value: selectionsBytes, Partition: kafka.PartitionAny, Topic: producerSynchronizationResultTopic}
		selectionSynchronizationJob := &model.Job{Data: message, IsCompletedChannel: make(chan bool)}
		kafkaProducerSynchronizationResultJobDispatcher.GetJobQueueChannel() <- selectionSynchronizationJob

		_, ok := <-selectionSynchronizationJob.IsCompletedChannel
		if ok {
			close(selectionSynchronizationJob.IsCompletedChannel)
		}
	})

	kafkaProducerSelectionSummaryJobDispatcher = dispatcher.NewDispatcher("producer", producerMaxWorker, producerMaxQueue, func(worker worker.IWorker, job model.IJob) {
		defer func() {
			if r := recover(); r != nil {
				fmt.Println("Recovered in Producer Task", r)
			}
			job.GetIsCompletedChannel() <- true
		}()

		message := job.GetData().(*consumer.CustomKafkaMessage)
		kafkaProducer.Produce(message, kafkaProducerMessageChannel, kafkaProducerErrorChannel)
		fmt.Println(fmt.Sprintf("producer workers %d is processing job : %v", worker.GetId(), job))
		time.Sleep(time.Millisecond * 1)
	})

	kafkaProducerSynchronizationResultJobDispatcher = dispatcher.NewDispatcher("producer", producerMaxWorker, producerMaxQueue, func(worker worker.IWorker, job model.IJob) {
		defer func() {
			if r := recover(); r != nil {
				fmt.Println("Recovered in Producer Task", r)
			}
			job.GetIsCompletedChannel() <- true
		}()

		message := job.GetData().(*consumer.CustomKafkaMessage)
		kafkaProducer.Produce(message, kafkaProducerMessageChannel, kafkaProducerErrorChannel)
		fmt.Println(fmt.Sprintf("producer workers %d is processing job : %v", worker.GetId(), job))
		time.Sleep(time.Millisecond * 1)
	})

	//region kafka consumer
	options := make(map[string]interface{})
	//options["config.consumer.offsets.initial"] = sarama.OffsetNewest
	kafkaConsumerProvider = &consumer.ConfluentKafkaConsumerProvider{KafkaConsumerProvider: &consumer.KafkaConsumerProvider{
		KafkaVersion:           "2.3.0",
		EnableThrottling:       false,
		MaxPendingMessageCount: 10,
		WaitingTimeMsWhenMaxPendingMessageCountReached: 50,
		PollTimeoutMS: 100,
		Options:       options,
	}}
	kafkaSelectionConsumer, kafkaConsumerProviderError = kafkaConsumerProvider.GetKafkaConsumer(broker, selectionConsumerGroup, []string{consumerSelectionTopic})

	if kafkaConsumerProviderError != nil {
		panic(kafkaConsumerProviderError)
	}

	kafkaSynchronizationConsumer, kafkaConsumerProviderError = kafkaConsumerProvider.GetKafkaConsumer(broker, synchronizationConsumerGroup, []string{consumerSynchronizationTopic})

	if kafkaConsumerProviderError != nil {
		panic(kafkaConsumerProviderError)
	}

	kafkaConsumerSelectionMessageChannel = make(chan interface{})
	kafkaConsumerSelectionErrorChannel = make(chan interface{})
	kafkaConsumerSelectionIgnoreChannel = make(chan interface{})
	kafkaConsumerSelectionSignalChannel = make(chan os.Signal, 1)
	signal.Notify(kafkaConsumerSelectionSignalChannel, syscall.SIGINT, syscall.SIGTERM)



	kafkaConsumerSynchronizationMessageChannel = make(chan interface{})
	kafkaConsumerSynchronizationErrorChannel = make(chan interface{})
	kafkaConsumerSynchronizationIgnoreChannel = make(chan interface{})
	kafkaConsumerSynchronizationSignalChannel = make(chan os.Signal, 1)
	signal.Notify(kafkaConsumerSynchronizationSignalChannel, syscall.SIGINT, syscall.SIGTERM)

	consumingLoop = true
	//endregion

	//region kafka producer
	kafkaProducerProvider = &producer.ConfluentKafkaProducerProvider{}
	kafkaProducer, kafkaProducerProviderError = kafkaProducerProvider.GetKafkaProducer(broker)

	if kafkaProducerProviderError != nil {
		panic(kafkaProducerProviderError)
	}

	kafkaProducerMessageChannel = make(chan interface{})
	kafkaProducerErrorChannel = make(chan interface{})
	kafkaProducerSignalChannel = make(chan os.Signal, 1)
	signal.Notify(kafkaProducerSignalChannel, syscall.SIGINT, syscall.SIGTERM)
	//endregion

}

func main() {

	s := server.NewServer()

	s.GET("/hello", func(c echo.Context) error {
		return c.String(http.StatusOK, "Hello, World!")
	})

	//region consumer

	s.GET("/consumer/job/counter", func(c echo.Context) error {
		return c.String(http.StatusOK, strconv.Itoa(cassandraJobDispatcher.GetJobCounter().GetCount()))
	})

	s.GET("/consumer/status", func(c echo.Context) error {
		return c.String(http.StatusOK, strconv.FormatBool(consumingLoop))
	})

	s.POST("/consumer/start", func(c echo.Context) error {
		consumingLoop = true
		return c.JSON(http.StatusOK, &struct {
			Message string `json:"message"`
		}{
			Message: "consumer dispatcher started successfully",
		})
	})

	s.POST("/consumer/stop", func(c echo.Context) error {
		consumingLoop = false
		return c.JSON(http.StatusOK, &struct {
			Message string `json:"message"`
		}{
			Message: "consumer dispatcher stopped successfully",
		})
	})

	//endregion

	quit := make(chan os.Signal)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM, os.Interrupt)

	defer cassandraSession.Close()

	wgMain := &sync.WaitGroup{}
	wgMain.Add(2)

	go func(_wgMain *sync.WaitGroup) {

		defer func() {
			_wgMain.Done()
		}()

		cassandraJobDispatcher.Run()
		postgresApiJobDispatcher.Run()
		kafkaProducerSelectionSummaryJobDispatcher.Run()
		kafkaProducerSynchronizationResultJobDispatcher.Run()

		wg := &sync.WaitGroup{}
		wg.Add(2)

		//kafka selection consumer routine
		go func(waitGroup *sync.WaitGroup) {
			kafkaSelectionConsumer.Consume(kafkaConsumerSelectionMessageChannel, kafkaConsumerSelectionErrorChannel, kafkaConsumerSelectionIgnoreChannel, cassandraJobDispatcher.MaxPendingJobCount)
			stoppedChannel := make(chan bool)
			go func() {
				defer func() {
					if r := recover(); r != nil {
						fmt.Println("Recovered in Consumer Routine", r)
					}
				}()

				defer func() {
					fmt.Println("consumer stopped")
					stoppedChannel <- true
				}()
				for {

					if !consumingLoop {
						select {
						case sgnl, ok := <-kafkaConsumerSelectionSignalChannel:
							if ok {
								fmt.Println("consumer signal (consumingLoop) : ", sgnl)
								return
							}
							continue
						default:
							time.Sleep(time.Millisecond * 1000)
							continue
						}
					}

					fmt.Println("consumer loop working...")

					select {
					case message := <-kafkaConsumerSelectionMessageChannel:

						//fmt.Println("message : ", message)
						job := &model.Job{
							Data:               message.(consumer.ICustomKafkaMessage).GetValue(),
							IsCompletedChannel: make(chan bool),
						}

						cassandraJobDispatcher.GetJobQueueChannel() <- job
						<-job.IsCompletedChannel
						close(job.IsCompletedChannel)
					case err := <-kafkaConsumerSelectionErrorChannel:
						fmt.Println("error : ", err)
					case ignore := <-kafkaConsumerSelectionIgnoreChannel:
						fmt.Println("ignore : ", ignore)
					case sgnl := <-kafkaConsumerSelectionSignalChannel:
						fmt.Println("consumer signal : ", sgnl)
						return
					}
				}
			}()
			<-stoppedChannel
			waitGroup.Done()
		}(wg)

		//endregion

		//kafka synchronization consumer routine
		go func(waitGroup *sync.WaitGroup) {
			kafkaSynchronizationConsumer.Consume(kafkaConsumerSynchronizationMessageChannel, kafkaConsumerSynchronizationErrorChannel, kafkaConsumerSynchronizationIgnoreChannel, cassandraJobDispatcher.MaxPendingJobCount)
			stoppedChannel := make(chan bool)
			go func() {
				defer func() {
					if r := recover(); r != nil {
						fmt.Println("Recovered in Consumer Routine", r)
					}
				}()

				defer func() {
					fmt.Println("consumer stopped")
					stoppedChannel <- true
				}()
				for {

					if !consumingLoop {
						select {
						case sgnl, ok := <-kafkaConsumerSynchronizationSignalChannel:
							if ok {
								fmt.Println("consumer signal (consumingLoop) : ", sgnl)
								return
							}
							continue
						default:
							time.Sleep(time.Millisecond * 1000)
							continue
						}
					}

					fmt.Println("consumer loop working...")

					select {
					case message := <-kafkaConsumerSynchronizationMessageChannel:

						//fmt.Println("message : ", message)
						job := &model.Job{
							Data:               message.(consumer.ICustomKafkaMessage).GetValue(),
							IsCompletedChannel: make(chan bool),
						}

						postgresApiJobDispatcher.GetJobQueueChannel() <- job
						<-job.IsCompletedChannel
						close(job.IsCompletedChannel)
					case err := <-kafkaConsumerSynchronizationErrorChannel:
						fmt.Println("error : ", err)
					case ignore := <-kafkaConsumerSynchronizationIgnoreChannel:
						fmt.Println("ignore : ", ignore)
					case sgnl := <-kafkaConsumerSynchronizationSignalChannel:
						fmt.Println("consumer signal : ", sgnl)
						return
					}
				}
			}()
			<-stoppedChannel
			waitGroup.Done()
		}(wg)

		//endregion

		wg.Wait()

		cassandraJobDispatcher.Stop()
		postgresApiJobDispatcher.Stop()
		kafkaProducerSelectionSummaryJobDispatcher.Stop()
		kafkaProducerSynchronizationResultJobDispatcher.Stop()
		fmt.Println("process finished...")

	}(wgMain)

	go func(_wgMain *sync.WaitGroup) {
		defer func() {
			_wgMain.Done()
		}()

		s.Serve(":8081", quit, 10*time.Second)
	}(wgMain)

	wgMain.Wait()
	fmt.Println("server stopped")
}
