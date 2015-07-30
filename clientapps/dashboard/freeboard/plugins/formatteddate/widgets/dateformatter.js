// # Building a Freeboard Plugin
//
// A freeboard plugin is simply a javascript file that is loaded into a web page after the main freeboard.js file is loaded.
//
// Let's get started with an example of a datasource plugin and a widget plugin.
//
// -------------------

// Best to encapsulate your plugin in a closure, although not required.
(function()
{
	// ## A Widget Plugin
	//
	// -------------------
	// ### Widget Definition
	//
	// -------------------
	// **freeboard.loadWidgetPlugin(definition)** tells freeboard that we are giving it a widget plugin. It expects an object with the following:
	freeboard.loadWidgetPlugin({
		// Same stuff here as with datasource plugin.
		"type_name"   : "dateformatter",
		"display_name": "Formatted date text",
        "description" : "Text widget with date format options",
		"fill_size" : true,
		"external_scripts" : [
			"plugins/formatteddate/js/moment-with-locales.min.js"
		],
		"settings"    : [
			{
				"name"        : "date",
				"display_name": "Date",
				// We'll use a calculated setting because we want what's displayed in this widget to be dynamic based on something changing (like a datasource).
				"type"        : "calculated"
			},
			{
				"name"		  : "format",
				"display_name": "Date format",
				"type"		  : "option",
				"options"	  :
					[
						{
							"name" : "Default",
							"value" : "default"
						},
						{
							"name" : "EN",
							"value" : "en"
						},
						{
							"name" : "NL",
							"value" : "nl"
						},
						{
							"name" : "DE",
							"value" : "de"
						},
						{
							"name" : "Custom format (fill in the Custom format field)",
							"value" : "custom"
						}
					],
				"default_value": "default"
			},
			{
				"name"        :"custom_format",
				"display_name":"Custom format",
				"description" :"Enter a custom date format according to the moment.js specification (see http://momentjs.com/docs/#/displaying/)"
			}

		],
		// Same as with datasource plugin, but there is no updateCallback parameter in this case.
		newInstance   : function(settings, newInstanceCallback)
		{
			console.log("Creating instance of plugin");
			newInstanceCallback(new formattedDateWidgetPlugin(settings));
		}
	});

	// ### Widget Implementation
	//
	// -------------------
	// Here we implement the actual widget plugin. We pass in the settings;
	var formattedDateWidgetPlugin = function(settings)
	{
		var self = this;
		var currentSettings = settings;

		var displayElement = $('<div class="tw-display"></div>');
		var valueElement = $('<div class="tw-value"></div>');
		var moment_locale;
		var moment_dateFormat;

		function updateValueSizing()
		{
			valueElement.css("max-width", "100%");
		}

		function getFormattedDate(formatSetting, customFormattingString, date)
		{
			if (formatSetting == "default")
			{
				moment.locale();
				return moment(date).format();
			}
			if (formatSetting == "custom")
			{
				return moment(date).format(customFormattingString);
			}

			moment.locale(formatSetting);
			return moment(date).format('LLLL');
		};


		// **render(containerElement)** (required) : A public function we must implement that will be called when freeboard wants us to render the contents of our widget. The container element is the DIV that will surround the widget.
		self.render = function (containerElement) {
			$(containerElement).empty();

			$(displayElement)
				.append($('<div class="tw-tr"></div>').append($('<div class="tw-value-wrapper tw-td"></div>').append(valueElement)));

			$(containerElement).append(displayElement);

			updateValueSizing();
		}

		// **getHeight()** (required) : A public function we must implement that will be called when freeboard wants to know how big we expect to be when we render, and returns a height. This function will be called any time a user updates their settings (including the first time they create the widget).
		//
		// Note here that the height is not in pixels, but in blocks. A block in freeboard is currently defined as a rectangle that is fixed at 300 pixels wide and around 45 pixels multiplied by the value you return here.
		//
		// Blocks of different sizes may be supported in the future.
		self.getHeight = function()
		{
			return 1;
		}

		var valueFontSize = 30;
		// **onSettingsChanged(newSettings)** (required) : A public function we must implement that will be called when a user makes a change to the settings.
		self.onSettingsChanged = function(newSettings)
		{
			// Normally we'd update our text element with the value we defined in the user settings above (the_text), but there is a special case for settings that are of type **"calculated"** -- see below.
			currentSettings = newSettings;

			valueElement.css({"font-size" : valueFontSize + "px"});
			updateValueSizing();

		}

		self.onSizeChanged = function()
		{
			updateValueSizing();
		}

		// **onCalculatedValueChanged(settingName, newValue)** (required) : A public function we must implement that will be called when a calculated value changes. Since calculated values can change at any time (like when a datasource is updated) we handle them in a special callback function here.
		self.onCalculatedValueChanged = function(settingName, newValue)
		{
			// Remember we defined "the_text" up above in our settings.
			if(settingName == "date")
			{
				var formattedDate = getFormattedDate(currentSettings.format,currentSettings.custom_format, newValue);
				valueElement.text(formattedDate);
			}

			updateValueSizing();
		}

		// **onDispose()** (required) : Same as with datasource plugins.
		self.onDispose = function()
		{
		}
	}
}());