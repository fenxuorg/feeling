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
    private static final int DEFAULT_INIT_SIZE = 10;
    private static final int WARNING_BAR = 100;
    private static final String emailTemplate = "{\"Messages\":[{\"From\":{\"Email\":\"t-chali@microsoft.com\",\"Name\":\"MailTest\"},\"To\":[{\"Email\":\"jipe@microsoft.com\",\"Name\":\"Test\"}],\"TemplateID\":489523,\"TemplateLanguage\":true,\"Subject\":\"Warning!AbnormalHeartRatse\",\"Variables\":{\"img_src\":\"https://steemitimages.com/0x0/http://ipfs.io/ipfs/QmTQo4cxDZ5MoszQAK93JyhFedeMuj7j4x5P7tQnvRi4A5\"}}]}";

    private int capacity = DEFAULT_INIT_SIZE;
    private RequestProxy storageServerProxy = new RequestProxy();
    private RequestProxy emailServerProxy = new RequestProxy("a217be7e96e9431486d65320add5f36a", "8b5b4330dae39d3657bfab6cc817719a");
    private Gson gson = new Gson();
    private int warning_count = 0;
    private boolean sending = false;

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

            boolean needSendWarning = warningComputer(heart_rate);
            if(needSendWarning) {
                // sendMsgToEmailService();
                if (heart_rates.size() != capacity) {
                    sendMsgToStorageServer();
                }
            }

            if (heart_rates.size() == capacity) {
                sendMsgToStorageServer();
            }
        }
        catch(Exception e) {
            Log.e("Error", e.toString());
        }
    }

    @Override
    public String toString(){
        return String.format("{\"user_id\":\"%s\",\"heart_rate\":%s}", this.user_id, gson.toJson(this.heart_rates));
    }

    private String GetTime(){
        Date date = new Date();
        SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd_HHmmss_SSS");
        return formatter.format(date);
    }

    private void sendMsgToStorageServer(){
        String json = this.toString();
        this.heart_rates.clear();
        Log.i("Request", "Send json to Storage server: "+ json);
        storageServerProxy.SendRequest(
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
                }
        );
    }

    private void sendMsgToEmailService(){
        Log.i("Request", "Send warning to Email service: ");
        emailServerProxy.SendRequest(
                new Request.Builder()
                        .post(RequestBody.create(MediaType.parse("application/json"), emailTemplate))
                        .url("https://api.mailjet.com/v3.1/send")
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

                        Log.i(" Server Request", "Email send: onResponse: " + response.body().string());
                    }
                }
        );
    }

    private boolean warningComputer(int heart_rate) {
        if(heart_rate>WARNING_BAR) {
            if(!sending) {
                warning_count++;
                if (warning_count >= 10) {
                    warning_count = 0;
                    sending = true;
                    return true;
                }
                return false;
            }
            return false;
        }else{
            sending = false;
            warning_count = 0;
            return false;
        }
    }
}
