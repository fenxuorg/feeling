package com.example.jipe.feeling;

import android.os.AsyncTask;
import android.util.Log;

import com.google.gson.Gson;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class DataProcessor extends AsyncTask<UserDataModel, String, String> {
    Gson gson;

    OkHttpClient client;

    public DataProcessor(){
        gson = new Gson();

        client = new OkHttpClient();
    }

    @Override
    protected String doInBackground(UserDataModel... data) {
        // String json = gson.toJson(data[0]);
        String json = "{	\"user_id\": \"1234\",	\"heart_rate\":{		\"2018-07-10T08:35:58.3873392Z\": 123,		\"2018-07-10T08:35:58.3873391Z\": 123,		\"2018-07-10T08:35:58.3873393Z\": 123,    }}";

        // If no internet, save to file
        // or send it to server
        Request request = new Request.Builder()
                .post(RequestBody.create(MediaType.parse("application/json"), json))
                .url("https://smartyi-webapp.azurewebsites.net/api/heartrate/receive")
                .build();

        Log.i("Request", "doInBackground: sending request" + request.toString());
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                e.printStackTrace();
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    throw new IOException("Server Error: " + response);
                }

                Log.i(" Server Request", "onResponse: " + response.body().string());
            }
        });

        return "";
    }
}
