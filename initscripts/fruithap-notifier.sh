#!/bin/sh
### BEGIN INIT INFO
# Provides:          fruithap-notifier
# Required-Start:    $local_fs $network $named $time $syslog rabbitmq-server
# Required-Stop:     $local_fs $network $named $time $syslog
# Default-Start:     2 3 4 5
# Default-Stop:      0 1 6
# Description:       A\\\ daemon\\\ that\\\ runs\\\ the\\\ fruithap\\\ engine
### END INIT INFO


RUNAS=maarten
NAME=fruithap-notifier
SCRIPTDIR=/home/$RUNAS/fruithap/notifier
SCRIPTNAME=doorbellnotifier.js

MIN_UPTIME="1000"
SPIN_SLEEP_TIME="10000"

LOGFILE=$SCRIPTDIR/$NAME.log   # maybe to /var/log
PIDFILE=$SCRIPTDIR/$NAME.pid   # maybe to /var/lock
APPLICATION_PATH=$SCRIPTDIR/$SCRIPTNAME
FOREVERCMD=$(which forever)
FOREVEROPTIONS="--pidFile $PIDFILE -a -o $LOGFILE -e $LOGFILE -l $LOGFILE --minUptime $MIN_UPTIME --spinSleepTime $SPIN_SLEEP_TIME start $APPLICATION_PATH"

start() {
  if [ -f $PIDFILE ] && kill -0 $(cat $PIDFILE); then
    echo "Service $NAME already running" >&2
    return 1
  fi
  echo "Starting service $NAME…" >&2  
  local CMD="$FOREVERCMD $FOREVEROPTIONS 2>&1 >> $LOGFILE & RETVAL=$?"
  exec $CMD
  echo "Service $NAME started" >&2
}

stop() {
  
  if [ -f $PIDFILE ]; then
        echo "Shutting down $NAME"
        # Tell Forever to stop the process.
        $FOREVERCMD stop $APPLICATION_PATH 2>&1 >> $LOGFILE        
        # Get rid of the pidfile, since Forever won't do that.
        rm -f $PIDFILE
        RETVAL=$?
    else
        echo "$NAME is not running."
        RETVAL=0
    fi
}

uninstall() {  
  echo "Uninstalling $NAME from init system"
  stop  
  update-rc.d -f $NAME remove      
}

installservice() {  
  echo "Installing $NAME into the init.d system..."
  update-rc.d $NAME defaults
}

status() {
    echo `forever list` | grep -q "$APPLICATION_PATH"
    if [ "$?" -eq "0" ]; then
        echo "$NAME is running."
        RETVAL=0
    else
        echo "$NAME is not running."
        RETVAL=3
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
