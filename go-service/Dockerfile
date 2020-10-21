FROM ubuntu

# Install the C lib for kafka
RUN apt-get update
RUN apt-get install -y --no-install-recommends apt-utils wget gnupg software-properties-common
RUN apt-get install -y apt-transport-https ca-certificates
RUN wget -qO - https://packages.confluent.io/deb/5.3/archive.key | apt-key add -
RUN add-apt-repository "deb [arch=amd64] https://packages.confluent.io/deb/5.3 stable main"
RUN apt-get update --fix-missing
#RUN apt-get install -y libssl1.1
#RUN apt-get install -y librdkafka1
RUN apt-get install -y librdkafka-dev

# Install Go
RUN add-apt-repository ppa:longsleep/golang-backports
RUN apt-get update
RUN apt-get install -y golang-1.15-go

# build the library
WORKDIR /go/src/github.com/resulguldibi/full-stack-sample/go-service
ADD . /go/src/github.com/resulguldibi/full-stack-sample/go-service

RUN GOPATH=/go GOOS=linux /usr/lib/go-1.15/bin/go build -a -o main .

EXPOSE 8000

ENTRYPOINT ["./main"]