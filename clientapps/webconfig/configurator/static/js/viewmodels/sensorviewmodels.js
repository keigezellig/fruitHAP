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

function SensorParameter(name, type, value){
    var self = this;
    self.name = ko.observable(name);
    self.type = ko.observable(type);
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
        if (typeof value == 'undefined' || value == "SELECT")
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
                        return new SensorParameter(item.Parameter, item.Type, "")
                    });
                    self.parameters(parameterList);
                }
            });
        }
    });



    self.save = function(formElement) {
        
        dataToSave = ko.toJSON({name: self.name, displayName: self.displayName, description: self.description, category: self.category, parameters: self.parameters, type: self.selectedSensorType })
        $.ajax(getApiUrl("configuration/sensors/add"), {
            data: dataToSave,
            type: "post", contentType: "application/json",
            success: function(result) { alert("Success! :"+ result) },
            error: function(errormsg) {self.errorsWhileSaving(true)}
        });
    };

    self.clearForm = function() {
        self.selectedSensorType("SELECT");
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
            self.availableSensorTypes.unshift("SELECT");
            self.selectedSensorType("SELECT");
        }
    });


}

ko.applyBindings(new SensorViewModel());