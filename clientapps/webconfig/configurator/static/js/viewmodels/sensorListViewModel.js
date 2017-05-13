function SensorItem(name, displayName, description, category, type) {
    var self = this;
    self.name = ko.observable(name);
    self.description = ko.observable(description);
    self.displayName = ko.observable(displayName);
    self.category = ko.observable(category);
    self.type = ko.observable(type);
}

function SensorListViewModel() {
    var self = this;
    self.sensors = ko.observableArray();
    $.ajax({
        type: "GET",
        url: getApiUrl("configuration/sensors/"),
        contentType: "text/javascript",
        dataType: 'jsonp',
        success: function (sensors) {
            var sensorList = $.map(sensors, function (item) {
                return new SensorItem(item.Name, item.DisplayName, item.Description, item.Category, item.Type);
            });

            self.sensors(sensorList);

        }
    });

    self.gridOptions = { data: self.sensors };

}

ko.applyBindings(new SensorListViewModel());