package com.example.jipe.feeling;

import java.util.concurrent.TimeUnit;

import okhttp3.Callback;
import okhttp3.OkHttpClient;
import okhttp3.Request;

public class RequestProxy {
    OkHttpClient client;

    public RequestProxy(){
        client = new OkHttpClient.Builder()
                .connectTimeout(5, TimeUnit.MINUTES) // connect timeout
                .writeTimeout(5, TimeUnit.MINUTES) // write timeout
                .readTimeout(5, TimeUnit.MINUTES) // read timeout;
                .build();
    }

    public void SendRequest(Request request, Callback callback){
        client.newCall(request).enqueue(callback);
    }
}
