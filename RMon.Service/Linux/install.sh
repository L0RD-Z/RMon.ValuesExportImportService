#!/bin/bash

SERVICE_NAME=$(awk -F "=" '/SERVICE_NAME/ {print $2}' service.info | tr -d '\r')
SERVICE_DISPLAY_NAME=$(awk -F "=" '/SERVICE_DISPLAY_NAME/ {print $2}' service.info| tr -d '\r')
SERVICE_EXE=$(awk -F "=" '/SERVICE_EXE/ {print $2}' service.info| tr -d '\r')

SERVICE_NAME="${1:-default}"$SERVICE_NAME

cat > ${SERVICE_NAME}.service <<EOL
[Unit]
Description=${SERVICE_DISPLAY_NAME}

[Service]
Type=notify
ExecStart=$PWD/${SERVICE_EXE}
ExecReload=/bin/kill -HUP \$MAINPID
StandardOutput=syslog

[Install]
WantedBy=multi-user.target
EOL

cp ${SERVICE_NAME}.service /etc/systemd/system/${SERVICE_NAME}.service 
systemctl enable ${SERVICE_NAME}

chmod a+rwx ./start.sh
chmod a+rwx ./stop.sh
chmod a+rwx ./status.sh
chmod a+rwx ./uninstall.sh
