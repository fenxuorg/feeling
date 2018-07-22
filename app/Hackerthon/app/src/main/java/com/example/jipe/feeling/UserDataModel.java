package com.example.jipe.feeling;

import android.util.Log;

import com.google.gson.Gson;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class UserDataModel {
    private static final int DEFAULT_INIT_SIZE = 2;
    private int capacity = DEFAULT_INIT_SIZE;
    private RequestProxy proxy = new RequestProxy();
    private Gson gson = new Gson();

    public String user_id;
    public HashMap<String, Integer> heart_rates;

    public UserDataModel(String userId){
        this(userId, DEFAULT_INIT_SIZE);
    }

    public UserDataModel(String userId, int capacity) {
        this.user_id = userId;
        this.capacity = capacity;
        this.heart_rates = new HashMap<String, Integer>();
    }

    public void add(int heart_rate) {
        try {
            String date = GetTime();
            this.heart_rates.put(date, heart_rate);
            Log.i("Info:", "add: user data: <" + date + ", " + heart_rate + ">, heart_rate.length:" + heart_rates.size());
            if (heart_rates.size() == capacity) {
                String json = this.toString();
                Log.i("Json", "Send: "+ json);
                proxy.SendRequest(
                        new Request.Builder()
                            .post(RequestBody.create(MediaType.parse("application/json"), json))
                            .url("https://smartyi-webapp.azurewebsites.net/api/heartrate/receive")
                            .build(),
                        new Callback() {
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
                this.heart_rates.clear();
            }
        }
        catch(Exception e) {
            Log.e("Error", e.toString());
        }
    }

    public String toString(){
        return String.format("{\"user_id\":\"%s\",\"heart_rate\":%s}", this.user_id, gson.toJson(this.heart_rates));
    }

    private String GetTime(){
        Date date = new Date();
        SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        return formatter.format(date);
    }
}
