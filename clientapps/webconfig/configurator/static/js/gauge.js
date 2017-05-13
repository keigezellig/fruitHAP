/**
 * Created by developer on 6/13/16.
 */

var gaugeOptions = {

        chart: {
            type: 'solidgauge',
            backgroundColor: 'transparent'
        },

        title: null,

        pane: {
            size: '65%',
            startAngle: -90,
            endAngle: 90,
            background: {
                innerRadius: '60%',
                outerRadius: '100%',
                shape: 'arc'
            }
        },

        // the value axis
        yAxis: {
            stops: [
                [0.1, '#55BF3B'], // green
                [0.5, '#DDDF0D'], // yellow
                [0.9, '#DF5353'] // red
            ],
            lineWidth: 0,
            minorTickInterval: null,
            tickPixelInterval: 400,
            tickWidth: 0,
            title: {
                y: -70
            },
            labels: {
                y: 16
            }
        },

        plotOptions: {
            solidgauge: {
                dataLabels: {
                    y: 5,
                    borderWidth: 0,
                    useHTML: true
                }
            }
        }
    };


    function loadSensors(dataUrl)
    {
        $.ajax({
            type: "GET",
            url: dataUrl,
            contentType: "text/javascript",
            dataType: 'jsonp',
            success: function(sensors)
            {
                for (var i=0;i < sensors.length;i++)
                {
                    valueType = sensors[i].ValueType;
                    valueUrl = sensors[i].SupportedOperations.GetValue;
                    containerId = 'gauge-'+sensors[i].Name;
                    displayName = sensors[i].DisplayName;

                    if (valueUrl && ( (valueType.indexOf("QuantityValue") > -1) || (valueType.indexOf("NumberValue") > -1)))
                    {
                        containerDiv = $("<div></div>");
                        containerDiv.attr('id',containerId);
                        columnDiv = $("<div></div>");
                        columnDiv.addClass("col-md-4");
                        columnDiv.append(containerDiv);
                        $("#dashboard").append(columnDiv);

                        generateGauge(containerId, valueUrl, sensors[i].Name, displayName, gaugeOptions);
                    }

                }

            }
        });


    }

    function generateHistory()
    {
        return new Highcharts.Chart({
            chart: {
                renderTo: 'history',
                defaultSeriesType: 'spline',
                events: {
                    load: function()
                    {
                        updateChartHistory(this)
                    }
                }
            },
            title: {
                text: 'Historical data'
            },
            xAxis: {
                type: 'datetime',
                tickInterval: 60000,
                //tickPixelInterval: 150,
                minRange: 20 * 1000
            },
            yAxis: {
                minPadding: 0.2,
                maxPadding: 0.2,
                title: {
                    text: 'Value',
                    margin: 5
                }
            },
            series: [{
                name: 'Sensor 1',
                data: [],
                marker: {
                    enabled: true
                }
            }]
        });
    }

    function generateGauge(renderToElement, dataUrl, sensorName, displayName, gaugeOptions)
    {
        Highcharts.setOptions(gaugeOptions);
        return new Highcharts.Chart({
            chart: {
                renderTo: renderToElement,
                events: {
                    load: function()
                    {
                        updateGauge(this, dataUrl)
                    }
                }
            },
            yAxis: {
                min: 0,
                max: 100,
                title: {
                    text: displayName
                }
            },

            credits: {
                enabled: false
            },

            series: [{
                name: sensorName,
                data: [0]

            }]

        });

    }

    function updateChartHistory(chart) {
        $.ajax({
            type: "GET",
            url: 'http://localhost:8888/api/sensor/FakeHum/GetValue',
            contentType: "text/javascript",
            dataType: 'jsonp',
            success: function(response) {
                var series = chart.series[0]
                var shift = series.data.length > 20; // shift if the series is
//                                                         // longer than 20

                if (response.Data.Content.Value)
                {
                    timestamp = new Date(response.TimeStamp).getTime();
                    value = response.Data.Content.Value.Value;
                    point = [timestamp, value];
                    chart.series[0].addPoint(point, true, shift);
                }

                // add the point


                // call it again after one second
                setTimeout(function(){
                    updateChartHistory(chart);
                },30000);
            },
            cache: false
        });
    }
    function updateGauge(gauge, dataUrl) {

        $.ajax({
            type: "GET",
            url: dataUrl,
            contentType: "text/javascript",
            dataType: 'jsonp',
            success: function(response) {
                if (gauge)
                {
                    var unit;
                    var value;

                    var axis = gauge.yAxis[0];
                    var series = gauge.series[0];
                    var point = series.points[0];


                    if (response.Data.TypeName.indexOf("QuantityValue") > -1)
                    {
                        if (response.Data.Content.Value)
                        {
                            unit = response.Data.Content.Value.UnitString;
                            value = response.Data.Content.Value.Value
                        }
                    }
                    else if (response.Data.TypeName.indexOf("NumberValue") > -1)
                    {
                        value = response.Data.Content.Value;
                    }

                    if (value)
                    {
                        dataLabelFormat = '<div style="text-align:center"><span style="font-size:25px;color:white"><b>{y}</b></span><br/>';
                    }
                    else
                    {
                        dataLabelFormat = '<div style="text-align:center"><span style="font-size:25px;color:white"><b>N/A</b></span><br/>';
                    }

                    if (unit)
                    {
                        dataLabelFormat += '<span style="font-size:12px;color:silver">'+unit+'</span></div>';
                    }

                    series.options.dataLabels.format = dataLabelFormat;

                    point.update(value);
                    setTimeout(function(){
                        updateGauge(gauge, dataUrl);
                    },4000);

                }
            },
            cache: false
        });                    
    }
