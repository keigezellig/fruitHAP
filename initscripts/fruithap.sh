#!/bin/sh
### BEGIN INIT INFO
# Provides:          fruithap
# Required-Start:    $local_fs $network $named $time $syslog rabbitmq-server
# Required-Stop:     $local_fs $network $named $time $syslog
# Default-Start:     2 3 4 5
# Default-Stop:      0 1 6
# Description:       A\\\ daemon\\\ that\\\ runs\\\ the\\\ fruithap\\\ engine
### END INIT INFO


RUNAS=maarten
NAME=fruithap
SERVICE_EXECUTABLE_NAME=FruitHAP.Startup.exe
MONOSERVICERUNNER=$(which mono-service)
SERVICE_EXECUTABLE_PATH=/home/$RUNAS/fruithap/
PIDFILE=/tmp/$SERVICE_EXECUTABLE_NAME.lock


start() {
  if [ -f $PIDFILE ] && kill -0 $(cat $PIDFILE); then
    echo "Service $NAME already running" >&2
    return 1
  fi
  echo "Starting service $NAME…" >&2
  cd $SERVICE_EXECUTABLE_PATH
  local CMD="$MONOSERVICERUNNER $SERVICE_EXECUTABLE_NAME & echo \$!"
  su -c "$CMD" $RUNAS
  echo "Service $NAME started" >&2
}

stop() {
  if [ ! -f "$PIDFILE" ] || ! kill -0 $(cat "$PIDFILE"); then
    echo 'Service $NAME not running' >&2
    return 1
  fi
  echo "Stopping service $NAME…" >&2
  kill -15 $(cat "$PIDFILE") && rm -f "$PIDFILE"
  echo "Service $NAME stopped" >&2
}

uninstall() {  
  echo "Uninstalling $NAME from init system"
  stop
  rm -f "$PIDFILE"    
  update-rc.d -f $NAME remove      
}

installservice() {  
  echo "Installing $NAME into the init.d system..."
  update-rc.d $NAME defaults
}

status() {
        printf "%-50s" "Checking service $NAME..."
    if [ -f $PIDFILE ]; then
        PID=$(cat $PIDFILE)
            if [ -z "$(ps axf | grep ${PID} | grep -v grep)" ]; then
                printf "%s\n" "The process appears to be dead but pidfile still exists"
            else    
                echo "Service $NAME is running, the PID is $PID"
            fi
    else
        printf "%s\n" "Service not running"
    fi
}


case "$1" in
  start)
    start
    ;;
  install)
    installservice
    ;;
  reinstall)
    uninstall
    installservice
    ;;
  stop)
    stop
    ;;
  status)
    status
    ;;
  uninstall)
    uninstall
    ;;
  restart)
    stop
    start
    ;;
  *)
    echo "Usage: $0 {start|stop|status|restart|uninstall}"
esac
