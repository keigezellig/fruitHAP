/*
 *  -----------------------------------------------------------------------------------------------
 *  "THE APPRECIATION LICENSE" (Revision 0x100):
 *  Copyright (c) 2016. M. Joosten
 *  You can do anything with this program and its code, even use it to run a nuclear reactor (why should you)
 *  But I won't be responsible for possible damage and mishap, because i never tested it on a nuclear reactor (why should I..)
 *  If you think this program/code is absolutely great and supercalifragilisticexpialidocious (or just plain useful), just
 *  let me know by sending me a nice email or postcard from your country of origin and leave this header in the code.
 *  Or better join my efforts to make this program even better :-)
 *  See my blog (http://joosten-industries/blog), for contact info
 *  ---------------------------------------------------------------------------------------------------
 *
 *
 */

function SensorParameter(name, type, value, allowedValues){
    var self = this;
    self.name = ko.observable(name);
    self.type = ko.observable(type);
    self.value = ko.observable(value);
    self.allowableValues = ko.observableArray(allowedValues);
}

function AllowableValue(name, value){
    var self = this;
    self.name = ko.observable(name);
    self.value = ko.observable(value);

}

function SensorViewModel() {
    var self = this;
    self.parameters = ko.observableArray();
    self.selectedSensorType = ko.observable("SELECT");
    self.name = ko.observable();
    self.displayName = ko.observable();
    self.description = ko.observable();
    self.category = ko.observable();
    self.availableSensorTypes = ko.observableArray();
    self.errorsWhileSaving = ko.observable();

    self.selectedSensorType.subscribe(function(value) {
        if (typeof value == 'undefined')
        {
            self.parameters([]);
        }
        else {
            $.ajax({
                type: "GET",
                url: getApiUrl("configuration/sensors/types/" + value + "?onlySpecific=true"),
                contentType: "text/javascript",
                dataType: 'jsonp',
                success: function (parameters) {

                    var parameterList = $.map(parameters, function (item) {
                        var sensorparam;
                        if (item.allowedValues != null) {
                            var allowedValueList = $.map(item.allowedValues, function(valueItem) {
                               return new AllowableValue(valueItem.Name, valueItem.Value)
                            });

                            sensorparam = new SensorParameter(item.Parameter, item.Type, "", allowedValueList);
                            alert(sensorparam.name() + "/" + sensorparam.value() + "/" + sensorparam.allowableValues().length );
                            return sensorparam
                        }
                        sensorparam = new SensorParameter(item.Parameter, item.Type, "", null);
                            alert(sensorparam.name() + "/" + sensorparam.value() + "/" + sensorparam.allowableValues() );
                        return sensorparam;

                    });
                    self.parameters(parameterList);
                }
            });
        }
    });



    self.save = function(formElement) {
        
        if (typeof self.selectedSensorType != 'undefined') {
            var dataToSave = ko.toJSON({
                Name: self.name,
                DisplayName: self.displayName,
                Description: self.description,
                Category: self.category,
                Parameters: self.parameters,
                Type: self.selectedSensorType
            })
            $.ajax(getApiUrl("configuration/sensors/add"), {
                data: dataToSave,
                type: "put", contentType: "application/json",
                success: function (result) {
                    self.errorsWhileSaving(false);
                    window.location='/restart';
                },
                error: function (errormsg) {
                    self.errorsWhileSaving(true);

                }
            });
        }
    };

    self.clearForm = function() {
        self.selectedSensorType(null);
        self.errorsWhileSaving(false);
        return true;
    };


    $.ajax({
        type: "GET",
        url: getApiUrl("configuration/sensors/types/"),
        contentType: "text/javascript",
        dataType: 'jsonp',
        success: function(types)
        {
            self.availableSensorTypes(types);
        }
    });


}

ko.applyBindings(new SensorViewModel());