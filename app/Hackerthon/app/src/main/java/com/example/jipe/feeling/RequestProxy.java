package com.example.jipe.feeling;

import okhttp3.Callback;
import okhttp3.OkHttpClient;
import okhttp3.Request;

public class RequestProxy {
    OkHttpClient client;

    public RequestProxy(){
        client = new OkHttpClient();
    }

    public void SendRequest(Request request, Callback callback){
        client.newCall(request).enqueue(callback);
    }
}
