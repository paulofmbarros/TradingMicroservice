#!/bin/bash

if kafka-topics --bootstrap-server kafka:9092 --list | grep -w "trade-executed"; then
  echo "Topic 'trade-executed' already exists."
else
  kafka-topics --create --topic trade-executed --bootstrap-server kafka:9092 --replication-factor 1 --partitions 1
  echo "Topic 'trade-executed' created."
fi
