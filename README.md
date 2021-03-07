---
title: Jyu.Sample.Kafka
tags: Github
description: 建置.net 5 console app 操作Kafka練習範例
---

# Jyu.Sample.Kafka

## About Docker

### Kafka UI – Free Web UI for Kafka
https://github.com/provectus/kafka-ui

Docker Run Command
```shell=
docker run -p 8080:8080 --restart always -e KAFKA_CLUSTERS_0_NAME=local -e KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS=kafka:9092 -d provectuslabs/kafka-ui:latest 
```

### Kafka - single-broker

kafka-docker
https://github.com/wurstmeister/kafka-docker

Dokcer Hub
https://hub.docker.com/r/wurstmeister/kafka/

* 啟動指令
```shell=
docker-compose up -d
```
* `docker-compose.yml` <br/>
https://github.com/s123600g/Jyu.Sample.Kafka/blob/main/docker-compose-single-broker/docker-compose.yml

# 

![](https://i.imgur.com/ydDWRQb.png)


