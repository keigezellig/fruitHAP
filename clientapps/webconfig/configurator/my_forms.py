import requests
from django.forms import *


def get_my_choices():
    req = requests.get("http://localhost/api/configuration/sensors")
    sensor_list = []
    response = req.json()

    for response_item in response:
        item = (response_item['Name'], response_item['Description'])
        sensor_list.append(item)

    return sensor_list


class SensorForm(Form):
    name = CharField(label='Name')
    displayName = CharField(label='Display name')
    description = CharField(label='Description', widget=Textarea)
    category = CharField(label='Category')

    def update_config(self):
        print(self.cleaned_data)

    def __init__(self, *args, **kwargs):
        veldjes = kwargs.pop('specific_fields')
        super(SensorForm, self).__init__(*args, **kwargs)

        for i, field in enumerate(veldjes):
            self.fields['custom_%s' % field[0]] = CharField(label=field[0])

        self.fields['sensorType'] = ChoiceField(
            choices=get_my_choices())
