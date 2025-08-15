#!/bin/bash
kubectl apply \
    -f ../student-management-system-namespace.yaml \
    -f ../rabbitmq.yaml \
    -f ../logserver.yaml \
    -f ../sqlserver.yaml \
    -f ../mailserver.yaml