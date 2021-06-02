#!/bin/bash

SERVICE_NAME=$(awk -F "=" '/SERVICE_NAME/ {print $2}' service.info | tr -d '\r')
SERVICE_NAME="${1:-default}"$SERVICE_NAME

systemctl start ${SERVICE_NAME}
