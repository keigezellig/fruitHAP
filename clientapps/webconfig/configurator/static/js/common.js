/**
 * Created by developer on 7/17/16.
 */
ko.bindingHandlers['reset'] = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        if (typeof valueAccessor() !== 'function')
            throw new Error('The value for a reset binding must be a function');

        ko.utils.registerEventHandler(element, 'reset', function (event) {
            var handlerReturnValue;
            var value = valueAccessor();

            try {
                handlerReturnValue = value.call(bindingContext['$data'], element);
            } finally {
                if (handlerReturnValue !== true) {
                    if (event.preventDefault)
                        event.preventDefault();
                    else
                        event.returnValue = false;
                }
            }
        });
    }
};


function getApiUrl(path)
{
    var API_BASE_URL = "/api/";

    return API_BASE_URL + path;
}
