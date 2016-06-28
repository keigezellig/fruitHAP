#!/bin/bash

python3.4 manage.py collectstatic
gunicorn webconfig.wsgi:application --bind 127.0.0.1:8001

