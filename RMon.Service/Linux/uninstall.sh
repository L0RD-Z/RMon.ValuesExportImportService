#!/bin/bash

SERVICE_NAME=$(awk -F "=" '/SERVICE_NAME/ {print $2}' service.info | tr -d '\r')
SERVICE_NAME="${1:-default}"$SERVICE_NAME

systemctl disable ${SERVICE_NAME}

rm /etc/systemd/system/${SERVICE_NAME}.service 
