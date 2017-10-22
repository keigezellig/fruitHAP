# -*- coding: utf-8 -*-
# Generated by Django 1.9.6 on 2016-08-01 16:46
from __future__ import unicode_literals

from django.db import migrations, models


class Migration(migrations.Migration):

    initial = True

    dependencies = [
    ]

    operations = [
        migrations.CreateModel(
            name='Site',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=20)),
                ('description', models.TextField(blank=True)),
                ('hostname', models.CharField(max_length=20)),
                ('configPort', models.IntegerField()),
                ('mqPort', models.IntegerField(blank=True)),
            ],
        ),
    ]