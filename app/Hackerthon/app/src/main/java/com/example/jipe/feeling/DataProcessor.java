package com.example.jipe.feeling;

import android.os.AsyncTask;
import android.util.Log;

import com.google.gson.Gson;

import java.io.IOException;

public class DataProcessor extends AsyncTask<UserDataModel, String, String> {
    private Gson gson = new Gson();

    @Override
    protected String doInBackground(UserDataModel... data) {
        String json = gson.toJson(data[0]);
        // If no internet, save to file
        // or send it to server

        return "";
    }
}
