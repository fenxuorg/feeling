package com.example.jipe.feeling;

import android.provider.ContactsContract;

import java.util.Date;
import java.util.HashMap;

public class UserDataModel {
    private static final int DEFAULT_INIT_SIZE = 20;

    private int capacity = DEFAULT_INIT_SIZE;

    public String user_id;
    public HashMap<Date, Integer> heart_rates;

    public UserDataModel(UserDataModel model) {
        this.user_id = model.user_id;
        this.heart_rates = model.heart_rates;
    }

    public UserDataModel(String userId){
        this(userId, DEFAULT_INIT_SIZE);
    }

    public UserDataModel(String userId, int capacity) {
        this.user_id = userId;
        this.capacity = capacity;
    }

    public void add(int heart_rate) {
        this.heart_rates.put(new Date(), heart_rate);
        if(heart_rates.size() == capacity) {
            new DataProcessor().execute(new UserDataModel(this));
            this.heart_rates.clear();
        }
    }
}
