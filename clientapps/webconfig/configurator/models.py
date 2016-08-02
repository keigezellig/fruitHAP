from django.db import models


# Create your models here.
class Site(models.Model):
    name = models.CharField('Name', max_length=20)
    description = models.TextField('Description', blank=True)
    hostname = models.CharField('Hostname', max_length=20)
    configPort = models.IntegerField('Configuration port')
    mqPort = models.IntegerField('Message queue port', blank=True)
