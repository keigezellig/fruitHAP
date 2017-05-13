#!/bin/bash

python3.5 manage.py collectstatic
gunicorn webconfig.wsgi:application --bind 127.0.0.1:8001 -w 3 --timeout 120

