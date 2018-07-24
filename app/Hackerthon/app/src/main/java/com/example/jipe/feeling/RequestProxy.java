package com.example.jipe.feeling;

import java.io.IOException;

import okhttp3.Authenticator;
import okhttp3.Callback;
import okhttp3.Credentials;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.Route;

public class RequestProxy {
    OkHttpClient client;

    public RequestProxy(){
        client = new OkHttpClient();
    }

    public RequestProxy(String username, String password){
        client = createAuthenticatedClient(username, password);
    }

    public void SendRequest(Request request, Callback callback){
        client.newCall(request).enqueue(callback);
    }

    private static OkHttpClient createAuthenticatedClient(final String username, final String password) {
        // build client with authentication information.
        OkHttpClient httpClient = new OkHttpClient.Builder().authenticator(new Authenticator() {
            public Request authenticate(Route route, Response response) throws IOException {
                String credential = Credentials.basic(username, password);
                return response.request().newBuilder().header("Authorization", credential).build();
            }
        }).build();
        return httpClient;
    }
}
