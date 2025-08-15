#!/bin/bash
kubectl delete svc --all -n student-management-system
kubectl delete deploy --all -n student-management-system
kubectl delete virtualservice --all -n student-management-system
kubectl delete destinationrule --all -n student-management-system