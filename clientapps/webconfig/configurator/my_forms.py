from django.forms import *


class SensorForm(Form):
    name = CharField(label='Name')
    displayName = CharField(label='Display name')
    description = CharField(label='Description', widget=Textarea)
    category = CharField(label='Category')
    

    def update_config(self):
        print("updating config....")
