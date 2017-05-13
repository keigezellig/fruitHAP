function SiteItem(name, hostname) {
    var self = this;
    self.name = ko.observable(name);
    self.hostname = ko.observable(hostname);
}

function SiteListViewModel() {
    var self = this;
    self.availableSites = ko.observableArray();


    $.ajax({
        type: "GET",
        url: configuration['configurator']['site_url'],
        contentType: "text/javascript",
        dataType: 'json',
        success: function (siteObject) {
            var siteList = $.map(siteObject.sites, function (item) {
                return new SiteItem(item.name, item.hostname);
            });

            self.availableSites(siteList);
        }
    });
}

ko.applyBindings(new SiteListViewModel());

