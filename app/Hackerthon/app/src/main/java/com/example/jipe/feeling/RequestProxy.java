package com.example.jipe.feeling;

import android.arch.core.util.Function;
import android.util.Log;

import org.apache.http.HttpResponse;
import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.concurrent.FutureCallback;
import org.apache.http.impl.nio.client.CloseableHttpAsyncClient;
import org.apache.http.impl.nio.client.HttpAsyncClients;

public class RequestProxy {
    private CloseableHttpAsyncClient httpclient;

    public RequestProxy(){
        RequestConfig requestConfig = RequestConfig.custom()
                .setSocketTimeout(3000).setConnectTimeout(3000).build();
        httpclient = HttpAsyncClients.custom()
                .setDefaultRequestConfig(requestConfig).build();

        try {
            httpclient.start();
        }
        catch (Exception e){
            Log.e("request", "RequestProxy: " + e.toString() );
        }
    }

    public void SendRequest(HttpUriRequest request, FutureCallback<HttpResponse> callback){
        httpclient.execute(request, callback);
    }
}
