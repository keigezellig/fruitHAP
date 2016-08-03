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
        url: "{% url 'site' %}",
        contentType: "text/javascript",
        dataType: 'json',
        success: function (sites) {
            var siteList = $.map(sites, function (item) {
                return new SiteItem(item.name, item.hostname);
            });

            self.availableSites(siteList);
        }
    });
}

ko.applyBindings(new SiteListViewModel());

