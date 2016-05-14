package com.fruithapnotifier.app.service;

import android.app.IntentService;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.os.ResultReceiver;
import android.util.Log;
import com.fruithapnotifier.app.common.Constants;
import okhttp3.*;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.MalformedURLException;
import java.net.URL;

public class RestConsumer extends IntentService
{
    private static final String TAG = RestConsumer.class.getName();

    public static final int GET = 0x1;
    public static final int POST = 0x2;
    public static final int PUT = 0x3;
    public static final int DELETE = 0x4;

    public static final String HTTP_VERB = "com.fruithapnotifier.app.service.restclient.HTTP_VERB";
    public static final String PARAMS = "com.fruithapnotifier.app.service.restclient.PARAMS";
    public static final String RESULT_RECEIVER = "com.fruithapnotifier.app.service.restclient.RESULT_RECEIVER";

    public RestConsumer()
    {
        super(TAG);
    }

    @Override
    protected void onHandleIntent(Intent intent)
    {
        // When an intent is received by this Service, this method
        // is called on a new thread.

        Uri uri = intent.getData();
        Bundle extras = intent.getExtras();

        if (extras == null || uri == null || !extras.containsKey(RESULT_RECEIVER))
        {
            // Extras contain our ResultReceiver and data is our REST action.  
            // So, without these components we can't do anything useful.
            Log.e(TAG, "You did not pass extras or data with the Intent.");

            return;
        }

        // We default to GET if no verb was specified.
        int verb = extras.getInt(HTTP_VERB, GET);
        Bundle params = extras.getParcelable(PARAMS);
        ResultReceiver receiver = extras.getParcelable(RESULT_RECEIVER);

        try
        {
            // Here we define our base request object which we will
            // send to our REST service via HttpClient.

            Request request = null;

            // Let's build our request based on the HTTP verb we were
            // given.
            switch (verb)
            {
                case GET:
                {
                    request = createGetRequest(uri, params);
                }
                break;

                case DELETE:
                {
                    request = createDeleteRequest(uri, params);
                }
                break;

                case POST:
                {
                    request = createPostRequest(uri, params);
                }
                break;

                case PUT:
                {
                    request = createPutRequest(uri, params);
                }
                default:
                    Log.e(TAG, "Incorrect verb");

                break;
            }


            if (request != null)
            {
                OkHttpClient client = new OkHttpClient();

                // Let's send some useful debug information so we can monitor things
                // in LogCat.
                Log.d(TAG, "Executing request: " + verbToString(verb) + ": " + uri.toString());

                // Finally, we send our request using HTTP. This is the synchronous
                // long operation that we need to run on this thread.
                Response response = client.newCall(request).execute();

                int statusCode = response.code();

                // Our ResultReceiver allows us to communicate back the results to the caller. This
                // class has a method named send() that can send back a code and a Bundle
                // of data. ResultReceiver and IntentService abstract away all the IPC code
                // we would need to write to normally make this work.

                Bundle resultData = new Bundle();

                if (response.body() != null)
                {
                    resultData.putString(Constants.REST_RESULT, response.body().string());
                }
                else
                {
                    resultData.putString(Constants.REST_RESULT, null);
                }

                receiver.send(statusCode, resultData);
            }
        }
        catch (UnsupportedEncodingException e)
        {
            Log.e(TAG, "A UrlEncodedFormEntity was created with an unsupported encoding.", e);
            Bundle resultData = new Bundle();
            resultData.putString(Constants.REST_RESULT,null);
            receiver.send(0, resultData);
        }

        catch (IOException e)
        {
            Log.e(TAG, "There was a problem when sending the request.", e);
            Bundle resultData = new Bundle();
            resultData.putString(Constants.REST_RESULT,null);
            receiver.send(0, resultData);
        }
    }


    private Request createGetRequest(Uri uri, Bundle params) throws MalformedURLException
    {
        Request request = new Request.Builder()
                .url(createUrl(uri, params))
                .addHeader("Content-type", "application/json")
                .get()
                .build();

        return request;
    }

    private Request createPostRequest(Uri uri, Bundle params) throws MalformedURLException
    {
        Request.Builder requestBuilder = new Request.Builder()
                .url(createUrl(uri, params))
                .post(createRequestBody(params));


        return requestBuilder.build();
    }

    private Request createPutRequest(Uri uri, Bundle params) throws MalformedURLException
    {
        Request.Builder requestBuilder = new Request.Builder()
                .url(createUrl(uri, params))
                .put(createRequestBody(params));

        return requestBuilder.build();
    }


    private Request createDeleteRequest(Uri uri, Bundle params) throws MalformedURLException
    {
        Request request = new Request.Builder()
                .url(createUrl(uri, params))
                .delete()
                .build();

        return request;
    }


    private URL createUrl(Uri uri, Bundle params) throws MalformedURLException
    {
        URL url;
        Uri.Builder uriBuilder = uri.buildUpon();

        if (params != null)
        {
            for (String key : params.keySet())
            {
                Object value = params.get(key);

                // We can only put Strings in a query string, so we call the toString()
                // method to enforce. We also probably don't need to check for null here
                // but we do anyway because Bundle.get() can return null.
                if (value != null)
                {
                    uriBuilder.appendQueryParameter(key, value.toString());
                }
            }
        }

        Uri resultUri = uriBuilder.build();
        url = new URL(resultUri.toString());
        return url;
    }

    private RequestBody createRequestBody(Bundle params)
    {
        if (params != null)
        {

            if (params.getString("JSONData") != null)
            {
                return RequestBody.create(MediaType.parse("application/json"),params.getString("JSONData"));
            }
            else
            {
                FormBody.Builder formBodyBuilder = new FormBody.Builder();
                if (params != null)
                {
                    for (String key : params.keySet())
                    {
                        Object value = params.get(key);

                        // We can only put Strings in a form body, so we call the toString()
                        // method to enforce. We also probably don't need to check for null here
                        // but we do anyway because Bundle.get() can return null.
                        if (value != null)
                        {
                            formBodyBuilder.add(key, value.toString());
                        }
                    }
                }

                return formBodyBuilder.build();
            }
        }

        return null;
    }

    private static String verbToString(int verb)
    {
        switch (verb)
        {
            case GET:
                return "GET";

            case POST:
                return "POST";

            case PUT:
                return "PUT";

            case DELETE:
                return "DELETE";
        }

        return "";
    }


    public static void executeRequest(Context context, int verb, Uri uri, Bundle parameters, ResultReceiver resultReceiver )
    {
        Intent intent = new Intent(context, RestConsumer.class);
        intent.setData(uri);
        intent.putExtra(HTTP_VERB,verb);
        intent.putExtra(PARAMS,parameters);
        intent.putExtra(RESULT_RECEIVER,resultReceiver);
        context.startService(intent);

    }


}
